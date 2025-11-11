using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles smooth transitions between Mixed Reality (MR) and Virtual Reality (VR) modes.
/// Fades the passthrough layer, toggles VR objects, and adjusts camera rendering.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VRFloorDetector _vrFloorDetector;
    [SerializeField] private Camera _cam;
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;
    [SerializeField] private GameObject _vrObjects;

    [Header("Settings")]
    [SerializeField, Tooltip("Duration of fade transition in seconds.")]
    private float _fadeDuration = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onSwitchedToVR;
    [SerializeField] private UnityEvent _onSwitchedToMR;

    private float _startOpacity;

    #region === TRANSITION TO VR ===
    /// <summary>
    /// Starts the transition from Mixed Reality to Virtual Reality.
    /// </summary>
    [ContextMenu("Fade To VR")]
    public void FadeToVR()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToTarget(0f, isToVR: true));
    }
    #endregion

    #region === TRANSITION TO MR ===
    /// <summary>
    /// Starts the transition from Virtual Reality back to Mixed Reality.
    /// </summary>
    [ContextMenu("Fade To MR")]
    public void FadeToMR()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToTarget(1f, isToVR: false));
    }
    #endregion

    #region === FADE CORE ===
    private IEnumerator FadeToTarget(float targetOpacity, bool isToVR)
    {
        _startOpacity = _passthroughLayer.textureOpacity;
        float elapsedTime = 0f;

        if (isToVR)
        {
            // Prepare for transition to VR
            _vrFloorDetector.enabled = true;
            _cam.clearFlags = CameraClearFlags.Skybox;
        }
        else
        {
            // Prepare for transition back to MR
            _vrFloorDetector.enabled = false;
            _cam.clearFlags = CameraClearFlags.SolidColor;
        }

        // Fade effect
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _fadeDuration;
            _passthroughLayer.textureOpacity = Mathf.Lerp(_startOpacity, targetOpacity, t);
            yield return null;
        }

        // Apply final state
        _passthroughLayer.textureOpacity = targetOpacity;

        if (isToVR)
        {
            _vrObjects.SetActive(true);
            _onSwitchedToVR?.Invoke();
        }
        else
        {
            _vrObjects.SetActive(false);
            _onSwitchedToMR?.Invoke();
        }
    }
    #endregion
}
