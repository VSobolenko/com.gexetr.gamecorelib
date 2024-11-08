using System;
using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class AliveTransition<TDataTransfer> : DeadTransition<TDataTransfer> 
{
    private readonly IEndState<TDataTransfer> _sourceState;
    private readonly IActivatedState<TDataTransfer> _targetState;

    private protected sealed override bool IsDecidedTransient => stateMachine.ActiveState == _sourceState;

    protected AliveTransition(IStateMachine stateMachine, IEndState<TDataTransfer> sourceState,
                              IActivatedState<TDataTransfer> targetState) : base(stateMachine, targetState)
    {
        _sourceState = sourceState;
        _targetState = targetState;
    }

    internal sealed override void Transit()
    {
        if (stateMachine.ActiveState == _targetState)
            throw new ArgumentException("Used directional transition for Active State. " +
                                        "Maybe you meant Circle Transition?");

        OnTransit();

        var transitionData = _sourceState.ReturnProcessedResult();
        _sourceState.Dispose();
        _targetState.ActivateState(stateMachine, transitionData);
        stateMachine.ChangeState(_targetState);
    }
}
}