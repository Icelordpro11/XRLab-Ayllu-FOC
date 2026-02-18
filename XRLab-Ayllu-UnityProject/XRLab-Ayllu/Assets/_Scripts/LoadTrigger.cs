using System;
using UnityEngine;

[ExecuteAlways]
public class LoadTrigger : MonoBehaviour
{
    [Header("References")]
    public Collider triggerCollider;

    public GameObject[] objectsToLoad;
    
    public GameObject[] objectsToUnload;

    [Header("Settings")]
    public Transform directionReference;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            SetObjectsActive(objectsToLoad, false);
        }
    }

    private void Reset()
    {
        directionReference = transform;

        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("MainCamera")) return;

        Vector3 playerPos = other.transform.position;
        Vector3 triggerPos = directionReference.position;
        Vector3 triggerForward = directionReference.forward;

        Vector3 toPlayer = (playerPos - triggerPos).normalized;
        float dot = Vector3.Dot(triggerForward, toPlayer);

        if (dot > 0f)
        {
            // FRONT side Load objects
            SetObjectsActive(objectsToLoad, true);
            SetObjectsActive(objectsToUnload, false);
            Debug.Log($"[LoadingTrigger] {name}: Exited on FRONT Loading objects");
        }
        else
        {
            // BACK side Unload objects
            SetObjectsActive(objectsToLoad, false);
            SetObjectsActive(objectsToUnload, true);
            Debug.Log($"[LoadingTrigger] {name}: Exited on BACK Unloading objects");
        }
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null) return;
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    private void OnDrawGizmos()
    {
        if (directionReference == null)
            directionReference = transform;
        
        Vector3 origin = directionReference.position;
        Vector3 forward = directionReference.forward;

        float arrowLength = 2f;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + forward * arrowLength);
        DrawArrowHead(origin + forward * arrowLength, forward, Color.green);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin - forward * arrowLength);
        DrawArrowHead(origin - forward * arrowLength, -forward, Color.red);
        
        if (triggerCollider != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            var box = triggerCollider as BoxCollider;
            if (box != null)
            {
                Matrix4x4 matrix = Matrix4x4.TRS(box.transform.position, box.transform.rotation, box.transform.lossyScale);
                Gizmos.matrix = matrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
        
        Gizmos.matrix = Matrix4x4.identity;
    }

    private void DrawArrowHead(Vector3 position, Vector3 direction, Color color)
    {
        Gizmos.color = color;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
        Gizmos.DrawLine(position, position + right * 0.5f);
        Gizmos.DrawLine(position, position + left * 0.5f);
    }
}
