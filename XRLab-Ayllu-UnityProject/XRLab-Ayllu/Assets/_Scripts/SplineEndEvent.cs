using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

/// <summary>
/// Invokes a UnityEvent when the attached SplineAnimate
/// finishes traversing its spline path.
/// </summary>
public class SplineEndEvent : MonoBehaviour
{
    [Tooltip("SplineAnimate component to monitor. If not assigned, it will be fetched automatically.")]
    [SerializeField] private SplineAnimate splineAnimate;

    [Tooltip("Event called when the spline animation completes.")]
    [SerializeField] private UnityEvent onSplineEndReached;

    private void OnEnable() => splineAnimate!.Completed += HandleSplineCompleted;

    private void OnDisable() => splineAnimate!.Completed -= HandleSplineCompleted;

    /// <summary>
    /// Called when the SplineAnimate completes its path.
    /// Invokes the UnityEvent when reaching the end of the spline.
    /// </summary>
    private void HandleSplineCompleted() => onSplineEndReached?.Invoke();
}

