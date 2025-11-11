using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles smooth transitions between Mixed Reality (MR) and Virtual Reality (VR) modes.
/// Fades the passthrough layer, toggles VR objects, and adjusts camera rendering.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private VRFloorDetector _vrFloorDetector;

    [SerializeField] private Camera _cam;
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;
    [SerializeField] private GameObject _vrObjects;

    [Header("Settings")] [SerializeField, Tooltip("Duration of fade transition in seconds.")]
    private float _fadeDuration = 1f;

    [Header("Events")] [SerializeField] private UnityEvent _onSwitchedToVR;
    [SerializeField] private UnityEvent _onSwitchedToMR;

    private float _startOpacity;

    public void FadeOut()
    {
        StartCoroutine(FadeToVR(0f)); // Fade opacity to 0 (fully transparent)
    }

    // Coroutine that gradually changes the passthrough layer opacity
    IEnumerator FadeToVR(float targetOpacity)
    {
        // while (!_spriteAnimation.IsFinished)
        // {
        //     _spriteAnimation.AdvanceAnimationFrame(); // Updates the current animation frame based on elapsed time.
        //     yield return null; // Wait until the next frame before continuing
        // }

        _startOpacity = _passthroughLayer.textureOpacity;
        float elapsedTime = 0f;

        // Gradually interpolate the opacity value over time
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _fadeDuration;

            // Smoothly interpolate between start and target opacity
            _passthroughLayer.textureOpacity = Mathf.Lerp(_startOpacity, targetOpacity, t);

            yield return null; // Wait for next frame
        }

        _onSwitchedToVR.Invoke();
        _vrObjects.SetActive(true);
        _cam.clearFlags = CameraClearFlags.Skybox; // Change Camera to VR
        _vrFloorDetector.enabled = true;
    }

    // Starts the fade-in coroutine (used to transition from VR to MR)
    public void FadeIn()
    {
        StartCoroutine(FadeToMR(1f)); // Fade opacity to 100% (fully visible)
    }

    // Coroutine that gradually changes the passthrough layer opacity
    private IEnumerator FadeToMR(float targetOpacity)
    {
        _startOpacity = _passthroughLayer.textureOpacity;
        float elapsedTime = 0f;

        // Gradually interpolate the opacity value over time (fade IN passthrough)
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _fadeDuration;

            // Smoothly interpolate between start and target opacity
            _passthroughLayer.textureOpacity = Mathf.Lerp(_startOpacity, targetOpacity, t);

            yield return null;
        }

        // Finalize transition back to MR
        _passthroughLayer.textureOpacity = targetOpacity;

        _vrObjects.SetActive(false);
        _cam.clearFlags = CameraClearFlags.SolidColor; // back to passthrough view
        _vrFloorDetector.enabled = false;

        _onSwitchedToMR?.Invoke();
    }
}
