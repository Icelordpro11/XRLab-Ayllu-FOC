using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text text;
    
    void Update()
    {
        text.text = "FPS: " + Mathf.Round(1f / Time.deltaTime).ToString();
    }
}