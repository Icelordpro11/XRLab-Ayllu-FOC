using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("If true, the animation will loop instead of stopping at the last frame.")]
    public bool CanLoop;

    [Tooltip("Time in seconds between frames.")]
    [SerializeField] private float _animSpeedPerFrame = 0.1f;

    [Header("References")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private List<Texture2D> _sprites;

    [Header("Events")]
    [Tooltip("Invoked when the animation finishes (only fires if CanLoop is false).")]
    public UnityEvent OnAnimationFinished;

    private Material _material;
    private int _currentFrame;
    private Coroutine _animationRoutine;

    /// <summary>
    /// True if the animation has reached the end and stopped.
    /// </summary>
    public bool IsFinished { get; private set; }

    [ContextMenu("Start Sprite Animation")]
    public void StartAnimation()
    {
        if (_animationRoutine != null)
            StopCoroutine(_animationRoutine);

        _animationRoutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        IsFinished = false;

        if (_meshRenderer != null && _material == null)
            _material = _meshRenderer.material;

        if (_sprites == null || _sprites.Count == 0 || _material == null)
            yield break;

        _currentFrame = 0;

        while (true)
        {
            UpdateMaterial();

            yield return new WaitForSeconds(_animSpeedPerFrame);

            _currentFrame++;

            // Check if animation reached the end
            if (_currentFrame < _sprites.Count) continue;
            if (CanLoop)
            {
                _currentFrame = 0;
                IsFinished = false;
            }
            else
            {
                _currentFrame = _sprites.Count - 1;
                IsFinished = true;
                OnAnimationFinished?.Invoke(); // 🔥 Fire UnityEvent
                break;
            }
        }

        _animationRoutine = null;
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
        if (_animationRoutine != null)
        {
            StopCoroutine(_animationRoutine);
            _animationRoutine = null;
        }

        IsFinished = true;
    }
}
