using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class SpriteAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("If true, the animation will loop instead of stopping at the last frame.")]
    [SerializeField] private bool _canLoop = false;

    [Tooltip("Time in seconds between frames.")]
    [SerializeField, Range(0.01f, 1f)] private float _animSpeedPerFrame = 0.1f;

    [Header("References")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private List<Texture2D> _sprites = new();

    [Header("Events")]
    [Tooltip("Invoked when the animation finishes (only fires if CanLoop is false).")]
    public UnityEvent OnAnimationFinished;

    private Material _material;
    private int _currentFrame;
    private CancellationTokenSource _cts;

    /// <summary>True if the animation has reached the end and stopped.</summary>
    public bool IsFinished { get; private set; }

    private void OnEnable() => StartAnimation().Forget();

    private void OnDisable() => StopAnimation();

    [ContextMenu("Start Sprite Animation")]
    public async UniTaskVoid StartAnimation()
    {
        StopAnimation(); // Ensure any previous animation is stopped

        if (_meshRenderer == null)
        {
            Debug.LogWarning($"[{nameof(SpriteAnimation)}] Missing MeshRenderer reference on {name}");
            return;
        }

        _material ??= _meshRenderer.material;

        if (_sprites == null || _sprites.Count == 0)
        {
            Debug.LogWarning($"[{nameof(SpriteAnimation)}] No sprites assigned on {name}");
            return;
        }

        _cts = new CancellationTokenSource();
        IsFinished = false;
        _currentFrame = 0;

        try
        {
            await PlayAnimationAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Animation was stopped or object disabled
        }
    }

    private async UniTask PlayAnimationAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            UpdateMaterial();

            await UniTask.Delay(TimeSpan.FromSeconds(_animSpeedPerFrame), cancellationToken: token);

            _currentFrame++;

            if (_currentFrame < _sprites.Count) continue;
            if (_canLoop)
            {
                _currentFrame = 0;
            }
            else
            {
                _currentFrame = _sprites.Count - 1;
                IsFinished = true;
                OnAnimationFinished?.Invoke();
                break;
            }
        }
    }

    private void UpdateMaterial()
    {
        if (_material == null || _sprites == null || _sprites.Count == 0)
            return;

        _currentFrame = Mathf.Clamp(_currentFrame, 0, _sprites.Count - 1);
        _material.mainTexture = _sprites[_currentFrame];
    }

    [ContextMenu("Stop Animation")]
    public void StopAnimation()
    {
        if (_cts == null)
            return;

        if (!_cts.IsCancellationRequested)
            _cts.Cancel();

        _cts.Dispose();
        _cts = null;
        IsFinished = true;
    }
}
