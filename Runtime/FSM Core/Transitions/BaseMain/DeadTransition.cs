using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class DeadTransition<TIn> : BaseTransition
{
    protected readonly IStateMachine stateMachine;
    private readonly IActivatedState<TIn> _targetState;

    internal DeadTransition(IStateMachine stateMachine, IActivatedState<TIn> state)
    {
        this.stateMachine = stateMachine;
        _targetState = state;
    }

    private protected abstract bool IsDecidedTransient { get; }

    internal sealed override bool Decide() => IsDecidedTransient && CanDecide();

    protected abstract bool CanDecide();
    protected virtual void OnTransit() { }
}
}