namespace Game.FSMCore.States
{
public abstract class State<TIn, TOut> : DeadState<TIn>, IEndState<TOut>
{
    TOut IEndState<TOut>.ReturnStateProcessedResult() => ReturnStateProcessedResult();

    protected abstract TOut ReturnStateProcessedResult();
}
}