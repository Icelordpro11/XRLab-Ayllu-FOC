using System;
using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    [SerializeField] private AudioClip[] ambienceClips;
    [SerializeField] private AudioSource audioSource;

    private int index;

    public void ChangeAmbience()
    {
        if (index < ambienceClips.Length)
        {
            index++;
            ambienceClips[index] = audioSource.clip;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}