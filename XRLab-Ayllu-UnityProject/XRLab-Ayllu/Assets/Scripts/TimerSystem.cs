using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TimerSystem : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField, Tooltip("How long the timer will run before invoking the event.")]
    private float _timerTime = 3f;

    [Header("Events")]
    [SerializeField, Tooltip("Invoked when the timer finishes.")]
    private UnityEvent OnTimerEnded;

    public bool IsRunning { get; private set; }

    private CancellationTokenSource _cts;

    [ContextMenu("Start Timer")]
    public async void StartTimer()
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
            await Task.Delay(TimeSpan.FromSeconds(_timerTime), _cts.Token);

            if (!_cts.IsCancellationRequested)
            {
                IsRunning = false;
                OnTimerEnded?.Invoke();
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
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