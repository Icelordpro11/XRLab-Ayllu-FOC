using System;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    [Header("layer")]
    [SerializeField] private LayerMask layer;
    [Header("events")]
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != layer) return;
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != layer) return;
        onTriggerExit.Invoke();
    }
}
