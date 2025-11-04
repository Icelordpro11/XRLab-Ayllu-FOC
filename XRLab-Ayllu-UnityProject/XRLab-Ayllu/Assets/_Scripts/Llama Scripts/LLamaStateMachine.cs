using UnityEngine;

public class LLamaStateMachine
{
    public LlamaState CurrentState;

    public void Initialize(LlamaState startState)
    {
        // called in start so current state doesn't start null
        CurrentState = startState;
        CurrentState.OnEnter();
    }

    public void ChangeState(LlamaState newState)
    {
        // calls exit on old state and enter on new state
        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }
}
