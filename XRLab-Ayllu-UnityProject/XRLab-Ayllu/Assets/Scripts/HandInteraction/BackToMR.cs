using UnityEngine;

public class BackToMR : MonoBehaviour
{
    private TransitionManager _transitionManager;

    void Start()
    {
        _transitionManager = FindAnyObjectByType<TransitionManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _transitionManager.FadeIn();
        }
    }
}
