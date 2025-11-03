using System;
using UnityEngine;

/// <summary>
/// Deprecated since we are going to be using another method of moving th player
/// </summary>
public class VRPlayerCollider : MonoBehaviour
{
    public CapsuleCollider collider;
    
    public Transform vrCameraTransform;
    public Transform xrOriginTransform;

    public float playerColliderRadius;
    public float headRoom;

    private void Update()
    {
        Vector3 floorHeight = new Vector3(0, xrOriginTransform.transform.position.y, 0);
        Vector3 vrCamPos = vrCameraTransform.position;

        Vector3 camDist = vrCamPos - transform.position;

        float colliderHeight = Mathf.Abs(floorHeight.y - vrCamPos.y);
        Debug.Log(colliderHeight);

        collider.center = new Vector3(camDist.x, colliderHeight * 0.5f, camDist.z);
        collider.height = colliderHeight;
        collider.radius = playerColliderRadius;
    }
}
