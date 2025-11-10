using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Preplays a Timeline (builds graph once) and pauses it immediately,
/// so you can resume later without performance spikes.
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class TimeLinePrewarmController : MonoBehaviour
{
    [Header("Director Reference")]
    [SerializeField] private PlayableDirector director;

    private bool _isPrepared;

    private void Awake()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        PrepareTimeline();
    }

    /// <summary>
    /// Builds the playable graph, plays once to initialize,
    /// then pauses immediately to avoid runtime spikes.
    /// </summary>
    public void PrepareTimeline()
    {
        if (_isPrepared) return;

        // Force rebuild & initialize
        director.RebuildGraph();
        director.time = 0;
        director.Play();
        director.Evaluate();

        // Pause so it’s ready to resume later
        director.Pause();

        _isPrepared = true;
    }

    /// <summary>
    /// Unpauses (resumes) the Timeline from its current time.
    /// </summary>
    [ContextMenu("Unpause Timeline")]
    public void UnpauseTimeline()
    {
        if (!_isPrepared)
            PrepareTimeline();

        director.Play();
    }

    /// <summary>
    /// Pauses the Timeline but keeps graph alive.
    /// </summary>
    [ContextMenu("Pause Timeline")]
    public void PauseTimeline()
    {
        if (!_isPrepared) return;
        director.Pause();
    }

    /// <summary>
    /// Restarts from the beginning without rebuilding the graph.
    /// </summary>
    [ContextMenu("Restart Timeline")]
    public void RestartTimeline()
    {
        if (!_isPrepared)
            PrepareTimeline();

        director.time = 0;
        director.Evaluate();
        director.Play();
    }
}
