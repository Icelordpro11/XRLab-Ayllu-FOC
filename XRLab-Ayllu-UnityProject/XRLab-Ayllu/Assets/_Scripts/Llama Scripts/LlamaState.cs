using UnityEngine;

public class LlamaState
{
    protected readonly Llama Llama;
    protected LLamaStateMachine LLamaStateMachine;

    // constructor method
    protected LlamaState(Llama llama, LLamaStateMachine llamaStateMachine)
    {
        this.Llama = llama;
        this.LLamaStateMachine = llamaStateMachine;
    }
    
    // called when entering state
    /// <summary>
    /// called when entering new state
    /// </summary>
    public virtual void OnEnter(){}
    
    // called when exiting state
    /// <summary>
    /// called when exiting state
    /// </summary>
    public virtual void OnExit(){}
    
    // called every frame state is active
    /// <summary>
    /// called every frame while state is active
    /// </summary>
    /// <param name="deltaTime">time since last frame</param>
    public virtual void OnUpdate(float deltaTime){}
}
