using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class CircularTransition<TIn, TOut> : DeadTransition<TIn>
{
    private readonly IEndState<TOut> _sourceState;
    private readonly IActivatedState<TIn> _targetState;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == _targetState;

    protected CircularTransition(IStateMachine stateMachine, State<TIn, TOut> state) : base(stateMachine, state)
    {
        _sourceState = state;
        _targetState = state;
    }

    internal sealed override void Transit()
    {
        OnTransit();

        _sourceState.ReturnStateProcessedResult();
        _sourceState.Dispose();
        _targetState.ActivateState(stateMachine, default);
        stateMachine.ChangeState(_targetState);
    }
}

public abstract class CircularCycleTransition<TTransferData> : DeadTransition<TTransferData>
{
    private readonly IEndState<TTransferData> _sourceState;
    private readonly IActivatedState<TTransferData> _targetState;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == _targetState;

    protected CircularCycleTransition(IStateMachine stateMachine, State<TTransferData, TTransferData> state) : base(
        stateMachine, state)
    {
        _sourceState = state;
        _targetState = state;
    }

    internal sealed override void Transit()
    {
        OnTransit();

        var result = _sourceState.ReturnStateProcessedResult();
        _sourceState.Dispose();
        _targetState.ActivateState(stateMachine, result);
        stateMachine.ChangeState(_targetState);
    }
}
}