using UnityEngine;
using Cysharp.Threading.Tasks;

public class LlamaApear : MonoBehaviour
{
    private static readonly int Dissapear = Shader.PropertyToID("_Dissappear");

    [Header("Material Settings")]
    [SerializeField] private Material[] _materials;

    [Header("Timing Settings")]
    [SerializeField, Range(0.1f, 5f)] private float _duration = 1f;

    /// <summary>
    /// Makes the llama gradually appear by adjusting the "_Dissappear" shader value.
    /// </summary>
    public async UniTask StartAppearing()
    {
        float start = 1f;
        float end = -0.9f;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(start, end, elapsed / _duration);

            foreach (var mat in _materials)
            {
                if (mat != null)
                    mat.SetFloat(Dissapear, value);
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        foreach (var mat in _materials)
        {
            if (mat != null)
                mat.SetFloat(Dissapear, -0.9f);
        }
    }

    [ContextMenu("Test StartAppearing")]
    private void TestStartAppearing()
    {
        StartAppearing().Forget();
    }
}