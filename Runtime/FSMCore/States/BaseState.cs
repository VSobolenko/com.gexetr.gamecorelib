using UnityEngine;

namespace Game.FSMCore.States
{
public abstract class BaseState : IState
{
    public virtual void UpdateState() { }

    public virtual void Finish() { }

    internal static int stateCounter;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCounter() => stateCounter = 0;
}
}