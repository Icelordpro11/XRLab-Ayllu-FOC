using UnityEngine;


public class CondorSystem : MonoBehaviour
{
    [SerializeField] private GameObject condorObj;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float condorOffset;

    public void RepositionCondor() => PositionCondor(cameraTransform, condorObj, condorOffset);

    /// <summary>
    /// Positions the specified condor above (or below) the VR camera
    /// by a given Y-axis offset.
    /// </summary>
    private void PositionCondor(Transform cameraPosition, GameObject condorObject, float verticalOffset)
    {
        if (cameraPosition == null || condorObject == null)
            return;

        Vector3 targetPosition = cameraPosition.position;
        targetPosition.y += verticalOffset;

        condorObject.transform.position = targetPosition;
    }

}