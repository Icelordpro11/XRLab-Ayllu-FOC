using System;
using UnityEngine;

public class SkyboxCaveChange : MonoBehaviour
{
    [SerializeField] private Material _Skybox_Day;
/// <summary>
/// Changes the skybox into the day material when the function gets called.
/// </summary>
    public void SkyboxStateChanger()
    {
        RenderSettings.skybox = _Skybox_Day;
    }
    
}
