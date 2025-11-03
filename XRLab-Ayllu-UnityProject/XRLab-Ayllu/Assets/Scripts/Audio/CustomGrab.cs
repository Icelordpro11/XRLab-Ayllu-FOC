using System;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class CustomGrab : MonoBehaviour
{
    [SerializeField] PointableElement _pointableElement;
    
    private bool IsGrabbed = false;
    
    public UnityEvent OnGrabbed = new UnityEvent();
    public UnityEvent OnRelease = new UnityEvent();

    private void Awake()
    {
        if (_pointableElement != null) return;
        _pointableElement = GetComponent<Grabbable>();
    }

    private void Update()
    {
        if (_pointableElement.SelectingPointsCount == 0)
        {
            if (IsGrabbed)
            {
                IsGrabbed = false;
                OnRelease?.Invoke();
            }
        }
        
        if (IsGrabbed) return;

        if (_pointableElement.SelectingPointsCount > 0)
        {
            IsGrabbed = true;
            OnGrabbed?.Invoke();
        }
    }
}
