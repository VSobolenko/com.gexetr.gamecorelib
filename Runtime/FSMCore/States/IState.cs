using System;

namespace Game.FSMCore.States
{
public interface IState : IDisposable
{
    void UpdateState();
}

public interface IQuiteState : IState
{
    internal void ActivateState(IStateMachine machine); // and protected
}

public interface IActivatedState<in TIn> : IState
{
    internal void ActivateState(IStateMachine machine, TIn data); // and protected
}

public interface IEndState<out TOut> : IState
{
    internal TOut ReturnStateProcessedResult(); // and protected
}
}