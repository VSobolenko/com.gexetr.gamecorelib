namespace Game.FSMCore.States
{
public abstract class State<TIn, TOut> : DeadState<TIn>, IEndState<TOut>
{
    TOut IEndState<TOut>.ReturnProcessedResult() => ReturnProcessedResult();
    
    protected abstract TOut ReturnProcessedResult();
}
}