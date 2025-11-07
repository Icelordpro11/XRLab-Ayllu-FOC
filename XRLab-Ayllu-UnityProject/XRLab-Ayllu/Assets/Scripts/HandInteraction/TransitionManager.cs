using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition MR and VR")]
    [SerializeField] private VRFloorDetector _vrFloorDetector;
    [SerializeField] private Camera _cam;
    [SerializeField] private OVRPassthroughLayer _passthroughLayer;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private GameObject _vrOjbects;
    [SerializeField] private SpriteAnimation _spriteAnimation;

    [SerializeField] private UnityEvent _onSwitchedEventToVR;
    [SerializeField] private UnityEvent _onSwitchedEventToMR;
    
    private float _startOpacity; // Used to store the initial opacity of the passthrough layer
    
    // Starts the fade-out coroutine (used to transition from MR to VR)
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
        
        _onSwitchedEventToVR.Invoke();
        _vrOjbects.SetActive(true);
        _cam.clearFlags = CameraClearFlags.Skybox; // Change Camera to VR
        _vrFloorDetector.enabled = true;
    }
    
    // Starts the fade-in coroutine (used to transition from VR to MR)
    public void FadeIn()
    {
        StartCoroutine(FadeToMR(1f)); // Fade opacity to 100% (fully visible)
    }

    // Coroutine that gradually changes the passthrough layer opacity
    IEnumerator FadeToMR(float targetOpacity)
    {
        _startOpacity = _passthroughLayer.textureOpacity;
        float elapsedTime = 0f;
        
        // Change Camera to MR
        _vrFloorDetector.enabled = false;
        _cam.clearFlags = CameraClearFlags.SolidColor;
        
        // Gradually interpolate the opacity value over time
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _fadeDuration;
            
            // Smoothly interpolate between start and target opacity
            _passthroughLayer.textureOpacity = Mathf.Lerp(_startOpacity, targetOpacity, t);
            
            yield return null;
        }
        _onSwitchedEventToMR.Invoke();
        _vrOjbects.SetActive(false);
    }
}
