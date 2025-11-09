using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// Controls fade-in (appear) and fade-out (disappear) transitions
/// for materials using a "_Dissappear" shader property.
/// </summary>
public class LlamaAppear : MonoBehaviour
{
    private static readonly int DissolveID = Shader.PropertyToID("_Dissappear");

    [Header("Material Settings")]
    [SerializeField] private Material[] _materials;

    [Header("Timing Settings")]
    [SerializeField, Range(0.1f, 5f)] private float _duration = 1f;

    /// <summary>
    /// Makes the llama gradually appear (fade in).
    /// </summary>
    public async UniTask StartAppearing()
    {
        await AnimateDissolve(1f, -0.9f);
    }

    /// <summary>
    /// Makes the llama gradually disappear (fade out).
    /// </summary>
    public async UniTask StartDisappearing()
    {
        await AnimateDissolve(-0.9f, 1f);
    }

    /// <summary>
    /// Shared function that interpolates the shader's dissolve property.
    /// </summary>
    private async UniTask AnimateDissolve(float startValue, float endValue)
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _duration);
            float value = Mathf.Lerp(startValue, endValue, t);

            foreach (var mat in _materials)
            {
                if (mat != null)
                    mat.SetFloat(DissolveID, value);
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        foreach (var mat in _materials)
        {
            if (mat != null)
                mat.SetFloat(DissolveID, endValue);
        }
    }

    [ContextMenu("Test Appear")]
    public void Appear() => StartAppearing().Forget();

    [ContextMenu("Test Disappear")]
    public void Disappear() => StartDisappearing().Forget();
}