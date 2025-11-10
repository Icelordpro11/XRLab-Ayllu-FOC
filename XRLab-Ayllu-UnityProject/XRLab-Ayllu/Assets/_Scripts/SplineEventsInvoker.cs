using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class SplineEventsInvoker : MonoBehaviour
{
    [SerializeField] private SplineAnimate splineAnimate;

    [Header("Events by Time (seconds)")]
    [SerializeField] private List<SplineEvent> events = new();

    private void Update()
    {
        if (splineAnimate == null || !splineAnimate.IsPlaying) return;

        float elapsed = splineAnimate.ElapsedTime;

        foreach (var e in events.Where(splineEvent => !splineEvent.triggered && elapsed >= splineEvent.time))
        {
            e.onReach?.Invoke();
            e.triggered = true;
        }
    }
}

[Serializable]
public class SplineEvent
{
    public float time;
    public UnityEvent onReach;
    [HideInInspector] public bool triggered;
}
