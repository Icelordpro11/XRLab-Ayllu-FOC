using System;
using UnityEngine;

public class VRFloorDetector : MonoBehaviour
{
    public LayerMask groundLayerMask;

    public Transform vrCameraTransform;
    public Transform xrOriginTransform;

    public float furtherGroundCheckLength = 0.5f;
    public float smoothSpeed = 5f;
    public float gravity = 9.81f;
    public float fallSpeed;

    private float _virtualGroundY;
    private bool _gravityEnabled;
    private float _startTime;
    private bool _isInPortal = false;

    private void Awake()
    {
        _startTime = Time.time;
        _virtualGroundY = xrOriginTransform.position.y;
    }

    private void LateUpdate()
    {
        //Don't update floor position while in portal
        if (_isInPortal)
        {
            return;
        }

        if (!_gravityEnabled && Time.time - _startTime >= 1f)
        {
            _gravityEnabled = true;
        }

        float xrOriginGroundDist = Mathf.Abs(xrOriginTransform.position.y - vrCameraTransform.position.y);

        // Main raycast
        if (Physics.Raycast(vrCameraTransform.position, Vector3.down, out RaycastHit groundHit, xrOriginGroundDist, groundLayerMask))
        {
            Debug.DrawRay(vrCameraTransform.position, Vector3.down * groundHit.distance, Color.red);

            _virtualGroundY = groundHit.point.y;
            fallSpeed = 0;
        }
        else if (Physics.Raycast(vrCameraTransform.position, Vector3.down, out RaycastHit furtherGroundHit, xrOriginGroundDist + furtherGroundCheckLength, groundLayerMask))
        {
            Debug.DrawRay(vrCameraTransform.position, Vector3.down * (xrOriginGroundDist + furtherGroundCheckLength), Color.blue);

            _virtualGroundY = furtherGroundHit.point.y;
            fallSpeed = 0;
        }
        else if (_gravityEnabled)
        {
            fallSpeed += gravity * Time.deltaTime;
            _virtualGroundY -= fallSpeed * Time.deltaTime;
        }

        float currentY = xrOriginTransform.position.y;
        float newY = Mathf.Lerp(currentY, _virtualGroundY, smoothSpeed * Time.deltaTime);

        xrOriginTransform.position = new Vector3(xrOriginTransform.position.x, newY, xrOriginTransform.position.z);
    }
    
    public void OnEnterPortal(Fragilem17.MirrorsAndPortals.Portal portal)
    {
        _isInPortal = true;
    }

    public void OnExitPortal(Fragilem17.MirrorsAndPortals.PortalableObject portalableObject, Fragilem17.MirrorsAndPortals.Portal portal)
    {
        _isInPortal = false;
        // Reset the virtual ground to current position when exiting portal
        _virtualGroundY = xrOriginTransform.position.y;
    }

    public void OnPostPortal(Fragilem17.MirrorsAndPortals.PortalableObject portalableObject, Fragilem17.MirrorsAndPortals.Portal portal)
    {
        // After teleporting through portal, update virtual ground position
        _virtualGroundY = xrOriginTransform.position.y;
    }
}