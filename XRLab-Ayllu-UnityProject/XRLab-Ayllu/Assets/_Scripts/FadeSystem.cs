using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FadeSystem : MonoBehaviour
{
   [Header("References")]
    [SerializeField] private GameObject fadeQuad;
    [SerializeField] private Renderer fadeRenderer;   // auto-found from fadeQuad if empty

    [Header("Settings")]
    [SerializeField, Min(0f)] private float fadeInDuration = 1f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent onFadeInComplete;   // alpha == 1
    [SerializeField] private UnityEvent onFadeOutComplete;  // alpha == 0

    private Material _mat;                // instance
    private int _colorId = -1;            // _BaseColor or _Color
    private CancellationTokenSource _cts;

    private void Awake()
    {
        if (!fadeRenderer && fadeQuad) fadeRenderer = fadeQuad.GetComponent<Renderer>();
        if (fadeRenderer) _mat = fadeRenderer.material; // instance, safe to edit

        // Prefer URP _BaseColor; fallback to _Color
        if (_mat != null)
        {
            var baseId = Shader.PropertyToID("_BaseColor");
            var colorId = Shader.PropertyToID("_Color");
            _colorId = _mat.HasProperty(baseId) ? baseId : colorId;
        }
    }

    private void OnDisable() => CancelFade();
    private void OnDestroy() => CancelFade();

    [ContextMenu("Fade In (alpha = 1)")]
    public void FadeIn()  => FadeToAlpha(1f, fadeInDuration).Forget();

    [ContextMenu("Fade Out (alpha = 0)")]
    public void FadeOut() => FadeToAlpha(0f, fadeOutDuration).Forget();

    public void CancelFade()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async UniTask FadeToAlpha(float targetAlpha, float duration)
    {
        if (_mat == null || fadeQuad == null || _colorId == -1)
        {
            Debug.LogWarning("[FadeQuadSystem] Missing material/quad or color property.");
            return;
        }

        CancelFade();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        fadeQuad.SetActive(true);

        float startAlpha = _mat.GetColor(_colorId).a;
        float elapsed = 0f;

        // instant case
        if (duration <= 0f)
        {
            SetAlpha(targetAlpha);
            if (Mathf.Approximately(targetAlpha, 0f)) fadeQuad.SetActive(false);
            InvokeEventFor(targetAlpha);
            return;
        }

        try
        {
            while (elapsed < duration)
            {
                token.ThrowIfCancellationRequested();
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float a = Mathf.Lerp(startAlpha, targetAlpha, t);
                SetAlpha(a);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            SetAlpha(targetAlpha);

            InvokeEventFor(targetAlpha);
        }
        catch (OperationCanceledException)
        {
            // interrupted
        }
    }

    private void SetAlpha(float a)
    {
        if (_mat == null || _colorId == -1) return;
        a = Mathf.Clamp01(a);
        var c = _mat.GetColor(_colorId);
        c.a = a;
        _mat.SetColor(_colorId, c);
    }

    private void InvokeEventFor(float targetAlpha)
    {
        if (Mathf.Approximately(targetAlpha, 1f)) onFadeInComplete?.Invoke();
        else if (Mathf.Approximately(targetAlpha, 0f)) onFadeOutComplete?.Invoke();
    }
}