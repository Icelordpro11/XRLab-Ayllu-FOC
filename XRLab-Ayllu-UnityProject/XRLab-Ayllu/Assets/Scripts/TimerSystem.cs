using System.Collections;
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

    private Coroutine _timerRoutine;

    /// <summary>
    /// True if the timer is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Starts a timer to invoke an event after X seconds where X = time.
    /// </summary>
    [ContextMenu("Start Timer")]
    public void StartTimer()
    {
        if (IsRunning)
        {
            Debug.LogWarning($"Timer is already running on {gameObject.name}");
            return;
        }

        _timerRoutine = StartCoroutine(Timer(_timerTime));
    }

    /// <summary>
    /// Stops the timer before it finishes.
    /// </summary>
    [ContextMenu("Stop Timer")]
    public void StopTimer()
    {
        if (_timerRoutine == null) return;
        StopCoroutine(_timerRoutine);
        _timerRoutine = null;
        IsRunning = false;
    }

    private IEnumerator Timer(float time)
    {
        IsRunning = true;
        yield return new WaitForSeconds(time);

        IsRunning = false;
        _timerRoutine = null;
        OnTimerEnded?.Invoke();
    }
}