using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CondorMountSystem : MonoBehaviour
{
    [Header("References (Assign Manually)")]
    [SerializeField] private Transform mountPosition; // Assign this in the inspector!
    [SerializeField] private SmoothSplineMovement splineMovementToActivate; // The spline movement to enable when flying
    
    [Header("Condor Swap")]
    [SerializeField] private GameObject condorToDisable; // The condor to disable (e.g., static/sitting condor)
    [SerializeField] private GameObject condorToEnable; // The condor to enable (e.g., flying condor)
    
    [Header("Additional Objects")]
    [SerializeField] private List<GameObject> objectsToDisableOnMount = new List<GameObject>(); // Objects to disable when mounting
    [SerializeField] private List<GameObject> objectsToEnableOnMount = new List<GameObject>(); // Objects to enable when mounting
    
    [Header("Mounting Settings")]
    [SerializeField] private float sitDuration = 2f; // Time to sit before mounting
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float stayWhiteDuration = 1f; // How long to stay white before fading out
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float delayBeforeFlight = 0.5f; // Short delay before flight starts
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // References (assigned at runtime)
    private Transform playerRig;
    private Transform vrCamera;
    private GameObject fadeQuad;
    private MeshRenderer fadeRenderer;
    private SittingZoneDetector sittingZone;
    private MonoBehaviour vrFloorDetector; // Reference to the VR Floor Detector script
    
    // State tracking
    private bool isMounted = false;
    private bool isMounting = false;
    private float timeInSittingZone = 0f;
    
    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private List<GameObject> disabledObjects = new List<GameObject>(); // Track what we disabled
    private List<GameObject> enabledObjects = new List<GameObject>(); // Track what we enabled
    
    private Material fadeMaterial;

    private void Awake()
    {
        AssignReferences();
    }

    private void AssignReferences()
    {
        // Find the player rig - looking for the Camera Rig which is the VR player root
        GameObject cameraRigObject = GameObject.Find("[BuildingBlock] Camera Rig");
        if (cameraRigObject == null)
        {
            // Fallback: search for any object with "Camera Rig" in the name
            cameraRigObject = FindObjectByNameContains("Camera Rig");
        }
        
        if (cameraRigObject != null)
        {
            playerRig = cameraRigObject.transform;
            DebugLog($"Player rig assigned: {playerRig.name}");
        }
        else
        {
            Debug.LogError("CondorMountSystem: Could not find Camera Rig! Make sure it exists in the scene.");
        }

        // Find the VR camera
        if (Camera.main != null)
        {
            vrCamera = Camera.main.transform;
            DebugLog($"VR Camera found: {vrCamera.name}");
        }
        else
        {
            Debug.LogError("CondorMountSystem: No main camera found!");
        }

        // Check mount position was assigned
        if (mountPosition == null)
        {
            Debug.LogError("CondorMountSystem: Mount Position not assigned! Please assign it in the inspector.");
        }
        else
        {
            DebugLog($"Mount position assigned: {mountPosition.name} at position {mountPosition.position}");
        }

        // Check spline movement assignment
        if (splineMovementToActivate == null)
        {
            Debug.LogWarning("CondorMountSystem: No Spline Movement assigned! Flight won't start unless you assign one.");
        }
        else
        {
            DebugLog($"Spline movement to activate: {splineMovementToActivate.gameObject.name}");
            splineMovementToActivate.enabled = false; // Make sure it starts disabled
        }

        // Check condor swap assignments
        if (condorToDisable != null)
        {
            DebugLog($"Condor to disable on mount: {condorToDisable.name}");
        }
        if (condorToEnable != null)
        {
            DebugLog($"Condor to enable on mount: {condorToEnable.name}");
            condorToEnable.SetActive(false); // Make sure it starts disabled
        }

        // Find the sitting zone detector - should be a child of this GameObject
        sittingZone = GetComponentInChildren<SittingZoneDetector>();
        if (sittingZone != null)
        {
            DebugLog($"Sitting zone detector found: {sittingZone.gameObject.name}");
        }
        else
        {
            Debug.LogError("CondorMountSystem: SittingZoneDetector not found in children!");
        }

        // Find or create the fade quad on the camera
        if (vrCamera != null)
        {
            // Look for existing FadeQuad
            Transform existingFade = vrCamera.Find("FadeQuad");
            if (existingFade != null)
            {
                fadeQuad = existingFade.gameObject;
                fadeRenderer = fadeQuad.GetComponent<MeshRenderer>();
                fadeMaterial = fadeRenderer.material;
                DebugLog("Found existing FadeQuad");
            }
            else
            {
                CreateFadeQuad();
            }
        }

        // Log objects to disable
        if (objectsToDisableOnMount.Count > 0)
        {
            DebugLog($"Objects to disable on mount: {objectsToDisableOnMount.Count}");
            foreach (GameObject obj in objectsToDisableOnMount)
            {
                if (obj != null)
                {
                    DebugLog($"  - {obj.name}");
                }
            }
        }

        // Log objects to enable
        if (objectsToEnableOnMount.Count > 0)
        {
            DebugLog($"Objects to enable on mount: {objectsToEnableOnMount.Count}");
            foreach (GameObject obj in objectsToEnableOnMount)
            {
                if (obj != null)
                {
                    DebugLog($"  - {obj.name}");
                }
            }
        }
    }

    private void CreateFadeQuad()
    {
        if (vrCamera == null)
        {
            Debug.LogError("CondorMountSystem: Cannot create fade quad - no camera found!");
            return;
        }

        DebugLog("Creating FadeQuad on camera...");

        // Create a quad in front of the camera
        fadeQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fadeQuad.name = "FadeQuad";
        fadeQuad.transform.SetParent(vrCamera, false); // Use worldPositionStays = false
        fadeQuad.transform.localPosition = new Vector3(0, 0, 0.3f); // Further from camera
        fadeQuad.transform.localRotation = Quaternion.identity;
        fadeQuad.transform.localScale = new Vector3(3f, 3f, 1f); // Bigger to cover entire view

        // Remove collider
        Collider col = fadeQuad.GetComponent<Collider>();
        if (col != null) Destroy(col);

        // Set up renderer and material
        fadeRenderer = fadeQuad.GetComponent<MeshRenderer>();
        
        // Try to find Unlit/Transparent shader first
        Shader fadeShader = Shader.Find("Unlit/Transparent");
        
        // Fallback to Unlit/Color if Transparent not found
        if (fadeShader == null)
        {
            fadeShader = Shader.Find("Unlit/Color");
            DebugLog("Using Unlit/Color shader");
        }
        else
        {
            DebugLog("Using Unlit/Transparent shader");
        }
        
        // Create new material with the shader
        fadeMaterial = new Material(fadeShader);
        fadeMaterial.color = new Color(1, 1, 1, 0); // Start transparent white
        
        // Enable transparency if using transparent shader
        if (fadeShader.name.Contains("Transparent"))
        {
            fadeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fadeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            fadeMaterial.SetInt("_ZWrite", 0);
            fadeMaterial.DisableKeyword("_ALPHATEST_ON");
            fadeMaterial.EnableKeyword("_ALPHABLEND_ON");
            fadeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            fadeMaterial.renderQueue = 3000;
        }
        
        fadeRenderer.material = fadeMaterial;
        fadeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        fadeRenderer.receiveShadows = false;
        
        fadeQuad.SetActive(false); // Start disabled
        
        DebugLog($"FadeQuad created. Shader: {fadeShader.name}, Local pos: {fadeQuad.transform.localPosition}, Scale: {fadeQuad.transform.localScale}");
    }

    private GameObject FindObjectByNameContains(string nameContains)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(nameContains))
            {
                return obj;
            }
        }
        return null;
    }

    private void Start()
    {
        if (fadeRenderer != null && fadeMaterial != null)
        {
            SetFadeAlpha(0f);
            fadeQuad.SetActive(false);
            DebugLog($"Fade system initialized. Material: {fadeMaterial.shader.name}");
        }
        else
        {
            Debug.LogWarning("CondorMountSystem: Fade Renderer or Material could not be assigned!");
        }

        DebugLog($"CondorMountSystem initialized on {gameObject.name}");
        DebugLog($"Sit Duration Required: {sitDuration} seconds");
    }

    private void Update()
    {
        // Check if player is in sitting zone and not already mounted/mounting
        if (sittingZone != null && sittingZone.IsPlayerInZone && !isMounted && !isMounting)
        {
            timeInSittingZone += Time.deltaTime;
            
            // Optional: Log progress every second
            if (enableDebugLogs && Mathf.FloorToInt(timeInSittingZone) != Mathf.FloorToInt(timeInSittingZone - Time.deltaTime))
            {
                DebugLog($"Sitting progress: {timeInSittingZone:F1}/{sitDuration} seconds");
            }
            
            if (timeInSittingZone >= sitDuration)
            {
                DebugLog($"Player sat for {sitDuration} seconds - starting mount!");
                StartMounting();
            }
        }
        else
        {
            // Reset timer if player leaves zone
            if (timeInSittingZone > 0)
            {
                DebugLog("Player left sitting zone - resetting timer");
            }
            timeInSittingZone = 0f;
        }
    }

    [ContextMenu("Start Mounting")]
    public void StartMounting()
    {
        if (isMounting || isMounted)
        {
            DebugLog($"Cannot start mounting: isMounting={isMounting}, isMounted={isMounted}");
            return;
        }

        if (mountPosition == null)
        {
            Debug.LogError("Cannot mount: Mount Position is not assigned!");
            return;
        }

        DebugLog("*** STARTING MOUNT SEQUENCE ***");
        StartCoroutine(MountSequence());
    }

    private IEnumerator MountSequence()
    {
        isMounting = true;
        timeInSittingZone = 0f; // Reset timer

        playerStartPosition = playerRig.position;
        playerStartRotation = playerRig.rotation;

        DebugLog($"Saved player start position: {playerStartPosition}");

        // 1. Fade to white
        DebugLog("Step 1: Fading to white...");
        yield return StartCoroutine(FadeToAlpha(1f, fadeInDuration));

        // 2. INSTANT teleport to mount position (happens while screen is white)
        DebugLog("Step 2: Instantly teleporting to condor...");
        TeleportPlayerToMount();

        // 3. Disable VR Floor Detector (find it just-in-time)
        DebugLog("Step 3: Disabling VR Floor Detector...");
        DisableVRFloorDetector();

        // 4. Swap condors
        DebugLog("Step 4: Swapping condors...");
        SwapCondors();

        // 5. Disable objects
        DebugLog("Step 5: Disabling objects...");
        DisableObjects();

        // 6. Enable objects
        DebugLog("Step 6: Enabling objects...");
        EnableObjects();

        // 7. Short delay before starting flight
        DebugLog($"Step 7: Starting flight in {delayBeforeFlight} seconds...");
        yield return new WaitForSeconds(delayBeforeFlight);

        // 8. Enable condor movement (while still white)
        DebugLog("Step 8: Starting flight!");
        if (splineMovementToActivate != null)
        {
            splineMovementToActivate.enabled = true;
            DebugLog($"Spline movement enabled on {splineMovementToActivate.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Spline movement reference is null!");
        }

        isMounted = true;

        // 9. Stay white while flying for a bit
        DebugLog($"Step 9: Flying while white for {stayWhiteDuration} seconds...");
        yield return new WaitForSeconds(stayWhiteDuration);

        // 10. Fade from white to transparent (while already flying)
        DebugLog("Step 10: Fading back to transparent...");
        yield return StartCoroutine(FadeToAlpha(0f, fadeOutDuration));

        isMounting = false;

        DebugLog("*** MOUNT SEQUENCE COMPLETE ***");
    }

    private void DisableVRFloorDetector()
    {
        // Find the VR Floor Detector just-in-time (when mounting)
        if (playerRig != null && vrFloorDetector == null)
        {
            // Try to find it by type name
            MonoBehaviour[] scripts = playerRig.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null && (script.GetType().Name == "VRFloorDetector" || script.GetType().Name.Contains("FloorDetector")))
                {
                    vrFloorDetector = script;
                    DebugLog($"  Found VR Floor Detector: {vrFloorDetector.GetType().Name}");
                    break;
                }
            }
        }

        // Disable it
        if (vrFloorDetector != null)
        {
            vrFloorDetector.enabled = false;
            DebugLog($"  VR Floor Detector disabled");
        }
        else
        {
            DebugLog("  VR Floor Detector not found on player rig - skipping");
        }
    }

    private void SwapCondors()
    {
        // Disable the sitting condor
        if (condorToDisable != null)
        {
            condorToDisable.SetActive(false);
            DebugLog($"  Disabled condor: {condorToDisable.name}");
        }

        // Enable the flying condor
        if (condorToEnable != null)
        {
            condorToEnable.SetActive(true);
            condorToEnable.gameObject.transform.position = vrCamera.position;
            DebugLog($"  Enabled condor: {condorToEnable.name}");
        }
    }

    private void SwapCondorsBack()
    {
        // Re-enable the sitting condor
        if (condorToDisable != null)
        {
            condorToDisable.SetActive(true);
            DebugLog($"  Re-enabled condor: {condorToDisable.name}");
        }

        // Disable the flying condor
        if (condorToEnable != null)
        {
            condorToEnable.SetActive(false);
            DebugLog($"  Disabled condor: {condorToEnable.name}");
        }
    }

    private void DisableObjects()
    {
        disabledObjects.Clear();
        
        foreach (GameObject obj in objectsToDisableOnMount)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
                disabledObjects.Add(obj);
                DebugLog($"  Disabled: {obj.name}");
            }
        }

        DebugLog($"Disabled {disabledObjects.Count} objects");
    }

    private void EnableObjects()
    {
        enabledObjects.Clear();
        
        foreach (GameObject obj in objectsToEnableOnMount)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                enabledObjects.Add(obj);
                DebugLog($"  Enabled: {obj.name}");
            }
        }

        DebugLog($"Enabled {enabledObjects.Count} objects");
    }

    private void ReDisableEnabledObjects()
    {
        DebugLog($"Re-disabling {enabledObjects.Count} objects...");
        
        foreach (GameObject obj in enabledObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                DebugLog($"  Re-disabled: {obj.name}");
            }
        }

        enabledObjects.Clear();
    }

    private void ReEnableDisabledObjects()
    {
        DebugLog($"Re-enabling {disabledObjects.Count} objects...");
        
        foreach (GameObject obj in disabledObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                DebugLog($"  Re-enabled: {obj.name}");
            }
        }

        disabledObjects.Clear();
    }

    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        if (fadeMaterial == null || fadeRenderer == null || fadeQuad == null)
        {
            Debug.LogWarning("Cannot fade: material, renderer, or quad is null");
            yield break;
        }

        fadeQuad.SetActive(true);
        
        float startAlpha = fadeMaterial.color.a;
        float elapsed = 0f;

        DebugLog($"Fading from {startAlpha:F2} to {targetAlpha:F2} over {duration} seconds");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetFadeAlpha(currentAlpha);
            
            // Debug log every 0.2 seconds to track fade progress
            if (enableDebugLogs && Mathf.FloorToInt(elapsed * 5) != Mathf.FloorToInt((elapsed - Time.deltaTime) * 5))
            {
                DebugLog($"  Fade progress: {(t * 100):F0}% (alpha: {currentAlpha:F2})");
            }
            
            yield return null;
        }

        SetFadeAlpha(targetAlpha);
        DebugLog($"Fade complete at alpha: {targetAlpha:F2}");

        if (targetAlpha == 0f)
        {
            fadeQuad.SetActive(false);
        }
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeMaterial == null) return;

        Color color = new Color(1, 1, 1, alpha); // White with variable alpha
        fadeMaterial.color = color;
    }

    private void TeleportPlayerToMount()
    {
        if (mountPosition == null || playerRig == null || vrCamera == null)
        {
            Debug.LogWarning("Cannot teleport player: mountPosition, playerRig, or vrCamera is null");
            return;
        }

        // Get the current offset from rig to camera (both position and rotation)
        Vector3 cameraLocalPosition = vrCamera.localPosition;
        Quaternion cameraLocalRotation = vrCamera.localRotation;
        
        DebugLog($"Camera local position to rig: {cameraLocalPosition}");
        DebugLog($"Camera local rotation to rig: {cameraLocalRotation.eulerAngles}");
        
        // Calculate world space offset
        Vector3 cameraOffsetWorld = vrCamera.position - playerRig.position;
        
        DebugLog($"Camera world offset from rig: {cameraOffsetWorld}");
        DebugLog($"Current camera world position: {vrCamera.position}");
        DebugLog($"Current rig world position: {playerRig.position}");
        DebugLog($"Mount target position: {mountPosition.position}");
        DebugLog($"Mount target rotation: {mountPosition.rotation.eulerAngles}");

        // To make the camera match the mount position exactly:
        // We need: vrCamera.position = mountPosition.position
        // Since: vrCamera.position = playerRig.position + playerRig.rotation * cameraLocalPosition
        // We need: playerRig.position = mountPosition.position - (mountPosition.rotation * cameraLocalPosition)
        
        Vector3 targetRigPosition = mountPosition.position - (mountPosition.rotation * cameraLocalPosition);
        Quaternion targetRigRotation = mountPosition.rotation * Quaternion.Inverse(cameraLocalRotation);

        DebugLog($"Calculated target rig position: {targetRigPosition}");
        DebugLog($"Calculated target rig rotation: {targetRigRotation.eulerAngles}");

        // INSTANT teleport - no lerping
        playerRig.position = targetRigPosition;
        playerRig.rotation = targetRigRotation;

        // Verify the camera is now at the mount position
        DebugLog($"After teleport - Camera position: {vrCamera.position} (should be {mountPosition.position})");
        DebugLog($"After teleport - Camera rotation: {vrCamera.rotation.eulerAngles} (should be {mountPosition.rotation.eulerAngles})");
        DebugLog($"Position error: {Vector3.Distance(vrCamera.position, mountPosition.position):F4} units");
        DebugLog($"Rotation error: {Quaternion.Angle(vrCamera.rotation, mountPosition.rotation):F2} degrees");

        // Parent player to condor
        playerRig.SetParent(mountPosition);
        
        DebugLog($"Player instantly teleported and parented to mount position.");
    }

    public void Dismount()
    {
        if (!isMounted) return;
        DebugLog("Starting dismount sequence");
        StartCoroutine(DismountSequence());
    }

    private IEnumerator DismountSequence()
    {
        // Disable spline movement
        if (splineMovementToActivate != null)
        {
            splineMovementToActivate.enabled = false;
            DebugLog("Spline movement disabled");
        }

        yield return StartCoroutine(FadeToAlpha(1f, fadeInDuration));

        playerRig.SetParent(null);

        // Instant teleport back
        playerRig.position = playerStartPosition;
        playerRig.rotation = playerStartRotation;

        // Swap condors back
        SwapCondorsBack();

        // Re-disable enabled objects
        ReDisableEnabledObjects();

        // Re-enable disabled objects
        ReEnableDisabledObjects();

        yield return StartCoroutine(FadeToAlpha(0f, fadeOutDuration));

        isMounted = false;
        DebugLog("Dismounted from condor");
    }

    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[CondorMount] {message}");
        }
    }

    // Public getters
    public bool IsMounted => isMounted;
    public float TimeInSittingZone => timeInSittingZone;
}