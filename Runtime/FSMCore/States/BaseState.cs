using System.Diagnostics;

namespace Game.FSMCore.States
{
[DebuggerNonUserCode]
public abstract class BaseState : IState
{
    public virtual void UpdateState() { }

    public virtual void Dispose() { }

    private protected static int stateCounter;
}
}