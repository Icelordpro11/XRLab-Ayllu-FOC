using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Invokes a UnityEvent automatically when the GameObject is enabled.
/// </summary>
public class OnEnableEventInvoker : MonoBehaviour
{
    [Header("Event Settings")]
    [Tooltip("This event is invoked automatically when the GameObject is enabled.")]
    public UnityEvent OnEnabledEvent;

    private void OnEnable() => OnEnabledEvent?.Invoke();
}