using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

/// <summary>
/// A UniTask-based timer system that waits for a set duration before invoking an event.
/// It supports cancellation, editor context menu control, and Unity lifecycle cleanup.
/// </summary>
public class TimerSystem : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField, Tooltip("How long the timer will run before invoking the event.")]
    private float _timerTime = 3f;

    [Header("Events")]
    [SerializeField, Tooltip("Invoked when the timer finishes.")]
    private UnityEvent OnTimerEnded;

    /// <summary>
    /// True if the timer is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }

    private CancellationTokenSource _cts;

    [ContextMenu("Start Timer")]
    public void StartTimer() => StartTimerAsync().Forget();

    /// <summary>
    /// Starts a UniTask timer that invokes the event after the specified duration.
    /// </summary>
    private async UniTaskVoid StartTimerAsync()
    {
        if (IsRunning)
        {
            Debug.LogWarning($"Timer already running on {gameObject.name}");
            return;
        }

        _cts = new CancellationTokenSource();
        IsRunning = true;

        try
        {
            // Wait using UniTask.Delay with cancellation
            await UniTask.Delay(TimeSpan.FromSeconds(_timerTime), cancellationToken: _cts.Token);

            if (!_cts.IsCancellationRequested)
                OnTimerEnded?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // Timer stopped manually
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
            IsRunning = false;
        }
    }

    [ContextMenu("Stop Timer")]
    public void StopTimer()
    {
        if (!IsRunning) return;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        IsRunning = false;
    }

    private void OnDisable()
    {
        StopTimer();
    }
}
