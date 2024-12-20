namespace Game.FSMCore.States
{
public abstract class QuiteState : BaseState, IQuiteState
{
    protected IStateMachine stateMachine;

    void IQuiteState.ActivateState(IStateMachine machine)
    {
        stateMachine = machine;
        OnStateActivated();
    }
    
    protected virtual void OnStateActivated() { }
}
}