using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class StackManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _requiredStacks = 1;        // Number of stacks required
    [SerializeField] private float _delayBeforeCompletion = 1f; // Wait time before event

    [Header("Events")]
    [SerializeField] private UnityEvent OnAllStacksCompleted;

    private int _currentStacks;

    /// <summary>
    /// Called whenever a stack is completed.
    /// </summary>
    public void AddCompletedStack()
    {
        _currentStacks++;

        if (_currentStacks >= _requiredStacks)
        {
            StartCompletionSequenceAsync().Forget();
        }
    }

    /// <summary>
    /// Waits for a short delay before invoking the completion event.
    /// </summary>
    private async UniTaskVoid StartCompletionSequenceAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeCompletion));
        OnAllStacksCompleted?.Invoke();
    }

    [ContextMenu("Start Completion Sequence")]
    public void StartCompletionSequence() => StartCompletionSequenceAsync().Forget();
}