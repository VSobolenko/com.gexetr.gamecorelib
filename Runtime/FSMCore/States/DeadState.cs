﻿namespace Game.FSMCore.States
{
public abstract class DeadState<TIn> : BaseState, IActivatedState<TIn>
{
    protected IStateMachine stateMachine;
    protected TIn inputData;

    void IActivatedState<TIn>.ActivateState(IStateMachine machine, TIn data)
    {
        inputData = data;
        stateMachine = machine;
        OnStateActivated();
    }

    protected virtual void OnStateActivated() { }
}
}