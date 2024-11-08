using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class CircularTransition<TIn, TOut> : DeadTransition<TIn>
{
    private readonly IEndState<TOut> _sourceState;
    private readonly IActivatedState<TIn> _targetState;
    
    //private readonly State<TIn, TOut> _state;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == _targetState;

    protected CircularTransition(IStateMachine stateMachine, State<TIn, TOut> state) : base(stateMachine, state)
    {
        //_state = state;
        _sourceState = state;
        _targetState = state;
    }

    internal sealed override void Transit()
    {
        OnTransit();

        _sourceState.ReturnProcessedResult();
        _sourceState.Dispose();
        _targetState.ActivateState(stateMachine, default);
        stateMachine.ChangeState(_targetState);
    }
}
}