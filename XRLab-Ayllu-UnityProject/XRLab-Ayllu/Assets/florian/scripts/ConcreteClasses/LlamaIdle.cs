using System.Linq;
using UnityEngine;

public class LlamaIdle : LlamaState
{
    
    public LlamaIdle(Llama llama, LLamaStateMachine llamaStateMachine) : base(llama, llamaStateMachine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Llama.animator.SetTrigger("idle");
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
