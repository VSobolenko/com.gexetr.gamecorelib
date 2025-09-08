using System;
using System.Collections.Generic;
using Game.FSMCore.States;

namespace Game.FSMCore.Machines
{
public interface IStateMachineOperator
{
    void Update();
    void StopMachine();
}

public sealed class LiteStateMachine : IStateMachine, IStateMachineOperator
{
    public IState ActiveState { get; private set; }
    public IState PreviousState { get; private set; }
    public event Action<IState> OnStateChange;

    void IStateMachine.ChangeState(IState newState) => SwitchState(newState);

    private readonly Dictionary<Type, IState> _states = new();

    public LiteStateMachine AddState<TState>(TState state) where TState : class, IState
    {
        IState boxedState = state; 
        if (_states.ContainsKey(boxedState.GetType()))
            throw new ArgumentException(nameof(boxedState), $"FSM contains {boxedState.GetType().Name} state");

        _states.Add(boxedState.GetType(), boxedState);
        return this;
    }

    public LiteStateMachine RemoveState<TState>(TState state) where TState : IState
    {
        _states.Remove(state.GetType());
        return this;
    }

    public void Update() => ActiveState?.UpdateState();

    public void StopMachine()
    {
        ActiveState.Finish();
        ActiveState = null;
    }

    public LiteStateMachine TransitTo<TState, TIn>(TIn data) where TState : class, IActivatedState<TIn> where TIn : class
    {
        IActivatedState<TIn> state = GetState<TState>();
        SwitchState(state);
        state.ActivateState(this, data);

        return this;
    }

    public LiteStateMachine TransitTo<TState>() where TState : class, IQuiteState
    {
        IQuiteState state = GetState<TState>();
        SwitchState(state);
        state.ActivateState(this);
        
        return this;
    }
    
    private void SwitchState(IState state)
    {
        PreviousState = ActiveState;
        ActiveState?.Finish();
        ActiveState = state;
        TryLogDebug(state);
        OnStateChange?.Invoke(state);
    }

    private TState GetState<TState>() where TState : class, IState
    {
        if (_states.TryGetValue(typeof(TState), out var state))
            return state as TState;

        throw new ArgumentNullException(nameof(TState), $"Unknown state type: {typeof(TState).Name}");
    }

    public IState this[Type type] => _states[type];
    
    public int Count => _states.Count;
        
    private bool _enableLogger;
    
    public LiteStateMachine EnableLogger()
    {
        _enableLogger = true;

        return this;
    }

    public LiteStateMachine DisableLogger()
    {
        _enableLogger = false;

        return this;
    }
    
    private void TryLogDebug(IState state)
    {
        if (_enableLogger == false)
            return;
        
        BaseState.stateCounter++;
        
        var stateName = state == null ? "Null_Or_Empty" : state.GetType().Name;
        var boxedCounter = BaseState.stateCounter.ToString();
        
        Log.Info($"[{boxedCounter}] Active state: {stateName}");
    }
}
}