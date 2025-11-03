using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayStoryAudio : MonoBehaviour
{
    [SerializeField] private UnityEvent doAfterAudio;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        _audioSource.Play();
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        doAfterAudio.Invoke();
    }
}
