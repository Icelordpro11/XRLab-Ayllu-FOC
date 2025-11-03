using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnece : MonoBehaviour
{
    [Header("layer")]
    [SerializeField] private LayerMask layer;
    
    [Header("event")]
    [SerializeField] private UnityEvent onTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != layer) return;
        onTrigger.Invoke();
        Destroy(gameObject);
    }
}