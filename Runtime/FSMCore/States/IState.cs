using System;

namespace Game.FSMCore.States
{
public interface IState : IDisposable
{
    void UpdateState();
}

public interface IQuiteState : IState
{
    protected internal void ActivateState(IStateMachine machine); // and protected
}

public interface IActivatedState<in TIn> : IState
{
    protected internal void ActivateState(IStateMachine machine, TIn data); // and protected
}

public interface IEndState<out TOut> : IState
{
    protected internal TOut ReturnStateProcessedResult(); // and protected
}
}