namespace Game.FSMCore.States
{
public abstract class BaseState : IState
{
    public virtual void UpdateState() { }

    public virtual void Dispose() { }

    internal static int stateCounter;
}
}