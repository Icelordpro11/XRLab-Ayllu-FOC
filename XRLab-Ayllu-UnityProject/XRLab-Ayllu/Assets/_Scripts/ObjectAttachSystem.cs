using UnityEngine;

public class ObjectAttachSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("The object that will be attached (e.g., CameraRig, PlayerRig).")]
    private Transform objectToAttach;

    [SerializeField, Tooltip("The target transform to attach to (e.g., Condor mount position).")]
    private Transform targetTransform;

    [Header("Settings")]
    [SerializeField, Tooltip("If true, keeps world position/rotation when attaching.")]
    private bool maintainWorldPosition = false;

    [SerializeField] private bool setAsParent;

    private Transform _originalParent;
    
    /// <summary>
    /// Attaches the reference object to the target transform.
    /// </summary>
    [ContextMenu("Attach To Target")]
    public void AttachToTarget()
    {
        if (objectToAttach == null || targetTransform == null)
            return;

        _originalParent = objectToAttach.parent;

        if (!setAsParent) return;
        objectToAttach.SetParent(targetTransform, maintainWorldPosition);
    }

    /// <summary>
    /// Detaches the object from the current target and restores its previous parent.
    /// </summary>
    [ContextMenu("Detach From Target")]
    public void DetachFromTarget()
    {
        if (objectToAttach == null)
            return;

        if (_originalParent != null)
            objectToAttach.SetParent(_originalParent, true);
        else
            objectToAttach.SetParent(null, true);
    }
}