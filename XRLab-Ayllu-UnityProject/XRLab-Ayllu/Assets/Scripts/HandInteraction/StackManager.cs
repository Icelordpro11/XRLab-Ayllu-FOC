using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StackManager : MonoBehaviour
{
    [SerializeField] private int _totalPacks; // Number of stone stacks required to trigger the llama summon
    [SerializeField] private ParticleSystem _summonParticle;
    [SerializeField] private GameObject _llama;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private UnityEvent OnRockStackComplete;
    
    private int CompletedStacks;
    
    public void AddCompletedStack()
    {
        CompletedStacks++; // Increment the number of finished stacks
        
        // Once all required stacks are completed, begin the summoning sequence
        if (CompletedStacks >= _totalPacks)
        {
            StartCoroutine("PrepareLlama"); // Start the coroutine that handles timing and effects
        }
    }

    // Coroutine that coordinates the summoning sequence timing
    private IEnumerator PrepareLlama()
    {
        yield return new WaitForSeconds(3);
        SummonParticle();
        
        // Calculate how long the particle system will take in total:
        // its duration plus the maximum lifetime of its particles
        float totalTime = _summonParticle.main.duration + _summonParticle.main.startLifetime.constantMax;
        
        yield return new WaitForSeconds(totalTime); // Wait until the particle effect has finished playing
        SummonLlama();
        _summonParticle.Stop();
    }
    
    private void SummonParticle()
    {
        _summonParticle.transform.position = _spawnPoint.position;
        _summonParticle.transform.rotation = _spawnPoint.rotation;
       // _summonParticle.Play();
    }
    
    private void SummonLlama()
    {
       // Instantiate(_llama, _spawnPoint.position, _spawnPoint.rotation);
        OnRockStackComplete.Invoke();
    }
}