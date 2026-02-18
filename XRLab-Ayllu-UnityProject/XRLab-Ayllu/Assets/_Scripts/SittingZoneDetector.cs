using UnityEngine;

public class SittingZoneDetector : MonoBehaviour
{
    private bool playerInSittingZone = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the main camera or if the root object has the camera
        if (other.CompareTag("MainCamera") || 
            other.transform.root.CompareTag("MainCamera") ||
            other.GetComponentInChildren<Camera>() != null ||
            other.transform.root.GetComponentInChildren<Camera>() != null)
        {
            playerInSittingZone = true;
            Debug.Log($"[SittingZone] Player entered sitting zone: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if it's the main camera or if the root object has the camera
        if (other.CompareTag("MainCamera") || 
            other.transform.root.CompareTag("MainCamera") ||
            other.GetComponentInChildren<Camera>() != null ||
            other.transform.root.GetComponentInChildren<Camera>() != null)
        {
            playerInSittingZone = false;
            Debug.Log($"[SittingZone] Player left sitting zone: {other.name}");
        }
    }

    public bool IsPlayerInZone => playerInSittingZone;
}