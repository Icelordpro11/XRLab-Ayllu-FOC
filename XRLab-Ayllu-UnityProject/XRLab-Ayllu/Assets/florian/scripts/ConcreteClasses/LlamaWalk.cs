using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class LlamaWalk : LlamaState
{
    public LlamaWalk(Llama llama, LLamaStateMachine llamaStateMachine) : base(llama, llamaStateMachine)
    {
    }

    public override void OnEnter()
    {
        // start spline animation to start following spline
        base.OnEnter();
        Llama.splineAnim.Play();
        Llama.animator.SetTrigger("walk");
    }

    public override void OnExit()
    {
        // stop spline animation to start idling 
        base.OnExit();
        Llama.splineAnim.Pause();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (Mathf.Approximately(Llama.splineAnim.ElapsedTime, Llama.splineAnim.Duration))
        {
            Llama.StopWalking();
        }
    }
}
