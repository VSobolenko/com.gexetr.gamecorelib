using System;
using System.Collections.Generic;
using System.Linq;
using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class Many2OneTransition<TDataTransfer> : DeadTransition<TDataTransfer>
{
    private readonly List<IEndState<TDataTransfer>> _sourceStates;
    private readonly IActivatedState<TDataTransfer> _targetState;

    private protected sealed override bool IsDecidedTransient => _sourceStates.Contains(stateMachine.ActiveState);

    protected Many2OneTransition(IStateMachine stateMachine, List<IEndState<TDataTransfer>> sourceStates,
                                 IActivatedState<TDataTransfer> targetState) : base(stateMachine, targetState)
    {
        _sourceStates = sourceStates;
        _targetState = targetState;
    }

    internal sealed override void Transit()
    {
        if (stateMachine.ActiveState == _targetState)
            throw new ArgumentException("Used directional transition for Active State. " +
                                        "Maybe you meant Circle Transition?");

        OnTransit();

        var transitionData = default(TDataTransfer);
        for (var i = 0; i < _sourceStates.Count; i++)
        {
            var data = _sourceStates[i].ReturnStateProcessedResult();
            _sourceStates[i].Finish();
            if (i == 0)
                transitionData = data;
        }

        _targetState.ActivateState(stateMachine, transitionData);
        stateMachine.ChangeState(_targetState);
    }
}
}