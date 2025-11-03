using UnityEngine;
using UnityEngine.Events;

public class TagTriggerSystem : MonoBehaviour
{
    [Header("Target Settings")] 
    [SerializeField] private string targetTag = "Player";

    [Header("Trigger Events")] 
    
    [SerializeField] private UnityEvent onTagEnter;
    [SerializeField] private UnityEvent onTagExit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        RPCOnTriggerEnter();
    }

    public void RPCOnTriggerEnter()
    {
        onTagEnter?.Invoke();
        Debug.Log("Tag Entered");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        RPCOnTriggerExit();
    }

    public void RPCOnTriggerExit()
    {
        onTagExit?.Invoke();
        Debug.Log("Tag Exit");
    }
}