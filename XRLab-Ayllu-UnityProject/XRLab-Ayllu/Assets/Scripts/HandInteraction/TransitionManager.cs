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
    [SerializeField] private GameObject _mrOjbects;
    [SerializeField] private GameObject _vrOjbects;
    [SerializeField] private SpriteAnimation _spriteAnimation;

    [SerializeField] private UnityEvent _onSwitchedEventToVR;
    [SerializeField] private UnityEvent _onSwitchedEventToMR;
    
    private float _startOpacity; // Used to store the initial opacity of the passthrough layer

    private void Start()
    {
        _vrFloorDetector = FindAnyObjectByType<VRFloorDetector>();
        _vrFloorDetector.enabled = false;
        _cam = Camera.main;
        _passthroughLayer = FindAnyObjectByType<OVRPassthroughLayer>();
        _vrOjbects.gameObject.SetActive(false);
    }
    
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
        _mrOjbects.SetActive(false);
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
        _mrOjbects.SetActive(true);
    }
}

// [Header("Light Sphere")]
// [SerializeField] private Transform _snapPoint;
// [SerializeField] private float _sphereSpeed;
// [SerializeField] private float _minDist; // Minimum distance threshold to stop moving the sphere

// // Coroutine that moves the sphere to the snap point smoothly
// public IEnumerator MoveSphere(Transform sphere)
// {
//     Vector3 endPos = _snapPoint.position;
//     
//     // Continue moving until the sphere is close enough to the target
//     while (Vector3.Distance(sphere.position, endPos) > _minDist)
//     {
//         // Move the sphere a small step toward the target each frame
//         sphere.position = Vector3.MoveTowards(
//             sphere.position,
//             endPos,
//             _sphereSpeed * Time.deltaTime
//         );
//         
//         yield return null; // Wait until the next frame before continuing
//     }
//     
//     FadeOut(); // Once the sphere reaches the snap point, start fading out MR visuals
// }