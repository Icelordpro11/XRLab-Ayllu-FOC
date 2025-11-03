using System;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class Llama : MonoBehaviour, IChangeState
{
    public SplineAnimate splineAnim;
    public Animator animator;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _clip;
    private int _index;
    
    #region StateMachine Variables
    
    public LLamaStateMachine stateMachine{get;set;}
    private LlamaIdle idle{get;set;}
    private LlamaWalk walk{get;set;}

    #endregion

    private void Awake()
    {
        // instantiate state machine and states
        stateMachine = new LLamaStateMachine();
        idle = new LlamaIdle(this, stateMachine);
        walk = new LlamaWalk(this, stateMachine);
        _source = GetComponent<AudioSource>();
        _source.clip = _clip[_index];
    }

    private void Start()
    {
        // initialize state machine
        stateMachine.Initialize(walk);
    }

    private void Update()
    {
        // call llama state update function
        stateMachine.CurrentState.OnUpdate(Time.deltaTime);
    }
    
    public void StartWalking()
    {
        stateMachine.ChangeState(walk);
        _index = Random.Range(0,_clip.Length);
        _source.Play();
    }

    public void StopWalking()
    {
        stateMachine.ChangeState(idle);
        _source.Stop();
    }
}
