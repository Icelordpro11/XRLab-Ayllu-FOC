using UnityEngine;

public class LightZone : MonoBehaviour
{
    private TransitionManager _transitionManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _transitionManager = FindAnyObjectByType<TransitionManager>();
    }

    private void OnTriggerEnter(Collider sphere)
    {
        if (!sphere.CompareTag("Grabbable")) return; // Only react if the object has the "Grabbable" tag
        
        // Disable physics on the sphere so it can be smoothly moved by script
        Rigidbody rb = sphere.attachedRigidbody;
        rb.isKinematic = true;
        
        // StartCoroutine(_tramsitionManager.MoveSphere(sphere.transform)); // Start moving the sphere toward the snap point
    }
}
