using UnityEngine;
using UnityEngine.Splines;

public class SmoothSplineMovement : MonoBehaviour
{
    [Header("Spline References")]
    [SerializeField] private SplineContainer splineContainer;

    [Header("VR Camera")]
    [SerializeField] private Transform vrCamera;

    [Header("Banking Object")]
    [SerializeField] private Transform bankingObject; // <- Assign lp_condor_FinalCopy here

    [Header("Base Movement")]
    [SerializeField] private float baseSpeed = 0.2f;
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private bool loop = true;

    [Header("Head Tilt Control")]
    [SerializeField] private float tiltSensitivity = 2f;
    [SerializeField] private float maxTiltAngle = 30f;
    [SerializeField] private float tiltDeadzone = 5f;
    [SerializeField] private AnimationCurve tiltResponseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Offset Limits")]
    [SerializeField] private float maxOffsetDistance = 3f;

    [Header("Plane Banking")]
    [SerializeField] private float bankingAmount = 25f;
    [SerializeField] private float bankingSmoothTime = 0.3f;

    [Header("Smoothing")]
    [SerializeField] private float offsetSmoothTime = 0.15f;
    [SerializeField] private float rotationSmoothTime = 0.4f;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    // Internal State
    private float currentT;
    private float targetOffsetAmount;
    private Vector3 currentOffset = Vector3.zero;
    private Vector3 offsetVelocity = Vector3.zero;
    private Quaternion currentRotation;
    private Vector3 rotationVelocity = Vector3.zero;
    private float currentBankAngle;
    private float bankVelocity;

    private Spline spline;

    // Store initial local position/rotation of banking object
    private Vector3 bankingObjectInitialLocalPosition;
    private Quaternion bankingObjectInitialLocalRotation;

    private void Start()
    {
        if (Camera.main != null)
        {
            vrCamera = Camera.main.transform;
            Debug.Log($"[SmoothSplineMovement] VR Camera found: {vrCamera.name}");
        }
        else
        {
            Debug.LogWarning("[SmoothSplineMovement] No main camera found! Head tilt controls will not work.");
        }
        
        currentRotation = transform.rotation;
        spline = splineContainer?.Spline;

        // Cache original local position/rotation for banking object
        if (bankingObject != null)
        {
            bankingObjectInitialLocalPosition = bankingObject.localPosition;
            bankingObjectInitialLocalRotation = bankingObject.localRotation;
        }
    }

    private void Update()
    {
        if (spline == null) return;

        UpdateSplineProgress();
        Vector3 splinePosition, splineForward, splineUp, splineRight;
        GetSplineReferencePoints(out splinePosition, out splineForward, out splineUp, out splineRight);

        float headTiltInput = GetHeadTiltInput();
        UpdateOffset(headTiltInput, splineRight);

        Vector3 targetPosition = splinePosition + currentOffset;
        transform.position = targetPosition;

        // Apply only base rotation to main object, banking to the child
        UpdateRotation(splineForward, splineUp, headTiltInput);
    }

    private void UpdateSplineProgress()
    {
        currentT += baseSpeed * speedCurve.Evaluate(currentT) * Time.deltaTime;
        currentT = loop ? Mathf.Repeat(currentT, 1f) : Mathf.Clamp01(currentT);
    }

    private void GetSplineReferencePoints(out Vector3 position, out Vector3 forward, out Vector3 up, out Vector3 right)
    {
        position = splineContainer.EvaluatePosition(currentT);
        forward = splineContainer.EvaluateTangent(currentT);
        up = splineContainer.EvaluateUpVector(currentT);
        right = Vector3.Cross(up, forward).normalized;
    }

    private void UpdateOffset(float headTiltInput, Vector3 splineRight)
    {
        targetOffsetAmount += -headTiltInput * tiltSensitivity * Time.deltaTime;
        targetOffsetAmount = Mathf.Clamp(targetOffsetAmount, -maxOffsetDistance, maxOffsetDistance);

        Vector3 targetOffset = splineRight * targetOffsetAmount;
        currentOffset = Vector3.SmoothDamp(currentOffset, targetOffset, ref offsetVelocity, offsetSmoothTime);
    }

    private void UpdateRotation(Vector3 splineForward, Vector3 splineUp, float tiltInput)
    {
        // Calculate base rotation (no banking)
        Quaternion baseRotation = Quaternion.LookRotation(splineForward, splineUp) * Quaternion.Euler(rotationOffset);

        // Smooth base rotation for main movement object
        currentRotation = SmoothDampRotation(currentRotation, baseRotation, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = currentRotation;

        // Banking only applied to bankingObject as Z rotation offset on initial local rotation
        if (bankingObject != null)
        {
            float targetBankAngle = tiltInput * bankingAmount;
            currentBankAngle = Mathf.SmoothDamp(currentBankAngle, targetBankAngle, ref bankVelocity, bankingSmoothTime);

            Quaternion bankRotation = Quaternion.Euler(0, 0, currentBankAngle);

            // Keep the original local pos/rot, just add the bank z-rotation
            bankingObject.localRotation = bankingObjectInitialLocalRotation * bankRotation;
            bankingObject.localPosition = bankingObjectInitialLocalPosition;
        }
    }

    private Quaternion SmoothDampRotation(Quaternion current, Quaternion target, ref Vector3 velocity, float smoothTime)
    {
        Vector3 currentEuler = current.eulerAngles;
        Vector3 targetEuler = target.eulerAngles;

        for (int i = 0; i < 3; i++)
        {
            float delta = targetEuler[i] - currentEuler[i];
            if (delta > 180f) targetEuler[i] -= 360f;
            else if (delta < -180f) targetEuler[i] += 360f;
        }

        Vector3 smoothedEuler = Vector3.SmoothDamp(currentEuler, targetEuler, ref velocity, smoothTime);
        return Quaternion.Euler(smoothedEuler);
    }

    private float GetHeadTiltInput()
    {
        if (vrCamera == null) return 0f;

        Vector3 cameraLocalEuler = vrCamera.localEulerAngles;
        float zRotation = cameraLocalEuler.z > 180f ? cameraLocalEuler.z - 360f : cameraLocalEuler.z;

        if (Mathf.Abs(zRotation) < tiltDeadzone) return 0f;

        float normalizedTilt = Mathf.Clamp(zRotation / maxTiltAngle, -1f, 1f);
        float curvedValue = tiltResponseCurve.Evaluate(Mathf.Abs(normalizedTilt));
        return Mathf.Sign(normalizedTilt) * curvedValue;
    }

    public float GetDistanceFromSpline() => currentOffset.magnitude;
    public float GetNormalizedDistanceFromSpline() => currentOffset.magnitude / maxOffsetDistance;
    public float GetCurrentTilt() => currentBankAngle;
}