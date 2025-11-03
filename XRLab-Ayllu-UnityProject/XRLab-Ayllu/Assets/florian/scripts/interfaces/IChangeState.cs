using UnityEngine;

public interface IChangeState
{
    /// <summary>
    /// call to make llama walk
    /// </summary>
    void StartWalking();
    /// <summary>
    /// call to make llama stop walking
    /// </summary>
    void StopWalking();
}
