using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("State Info")]
    [SerializeField] private GameState _currentState = GameState.Onboarding;
    
    public void SetState(GameState newState)
    {
        _currentState = newState;
        Debug.Log($"Game state changed to: {_currentState}");
    }
    
    #region State Setters
    public void SetState_GameStart() => SetState(GameState.GameStart);
    public void SetState_Cross() => SetState(GameState.Cross);
    public void SetState_MRToVR() => SetState(GameState.MR_TO_VR);
    public void SetState_StoneStacking() => SetState(GameState.StoneStacking);
    public void SetState_FollowLLama() => SetState(GameState.FollowLLama);
    public void SetState_Cave() => SetState(GameState.Cave);
    public void SetState_TopMountain() => SetState(GameState.TopMountain);
    #endregion
}

/// <summary>
/// Defines all possible audio/game phases.
/// </summary>
public enum GameState
{
 Onboarding,
 GameStart,
 Cross,
 MR_TO_VR,
 StoneStacking, 
 FollowLLama,
 Cave, 
 TopMountain, 
}