using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using Random = UnityEngine.Random;

public class RockSFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] rockSFX;
    [SerializeField] private AudioSource[] audioSource;
    private int randomSFX;
    private int[] audioSourceindex;
    private int sourceindex;

    [ContextMenu("playsound")]

    public void PlaySound()
    {
        randomSFX = Random.Range(0, rockSFX.Length);
       audioSource[sourceindex].clip = rockSFX[randomSFX];
       audioSource[sourceindex].Play();
    }
}
