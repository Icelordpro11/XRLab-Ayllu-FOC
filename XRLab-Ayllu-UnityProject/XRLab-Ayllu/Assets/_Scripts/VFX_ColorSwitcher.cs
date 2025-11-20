using UnityEngine;
using UnityEngine.VFX;
using Cysharp.Threading.Tasks;

public class VFX_ColorSwitcher : MonoBehaviour
{
    [SerializeField] private VisualEffect _vfx;
    [SerializeField] private string _colorProperty = "Color";

    [Header("Colors")]
    [SerializeField, ColorUsage(true, true)]
    private Color _normalColor = Color.white;

    [SerializeField, ColorUsage(true, true)]
    private Color _targetColor = Color.red;

    private int _colorId;

    private void Awake()
    {
        _vfx ??= GetComponent<VisualEffect>();
        _colorId = Shader.PropertyToID(_colorProperty);

        // Optional: sync normal color from VFX start value
        _normalColor = _vfx.GetVector4(_colorId);
    }

    [ContextMenu("Change Color Target")]
    // Call from UnityEvent: ChangeToTargetColor(1f);
    public void ChangeToTargetColor(float duration)
    {
        FadeTo(_targetColor, duration).Forget();
    }

    [ContextMenu("Change Color Normal")]
    // Call from UnityEvent: ChangeToNormalColor(1f);
    public void ChangeToNormalColor(float duration)
    {
        FadeTo(_normalColor, duration).Forget();
    }

    private async UniTask FadeTo(Color end, float duration)
    {
        Color start = (Color)_vfx.GetVector4(_colorId);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = t / duration;

            _vfx.SetVector4(_colorId, (Vector4)Color.Lerp(start, end, blend));

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        _vfx.SetVector4(_colorId, (Vector4)end);
    }
}