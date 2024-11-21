using System;
using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class EntryTransition<TIn> : DeadTransition<TIn>
{
    private readonly TIn _inputData;
    private readonly IActivatedState<TIn> _targetState;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == null;

    protected EntryTransition(IStateMachine stateMachine, TIn inputData,
                              IActivatedState<TIn> targetState) : base(stateMachine, targetState)
    {
        _inputData = inputData;
        _targetState = targetState;
    }

    internal sealed override void Transit()
    {
        if (stateMachine.ActiveState == _targetState)
            throw new ArgumentException("Used directional transition for Active State. " +
                                        "Maybe you meant Circle Transition?");

        OnTransit();
        _targetState.ActivateState(stateMachine, _inputData);
        stateMachine.ChangeState(_targetState);
    }

    protected sealed override bool CanDecide() => true;
}
}