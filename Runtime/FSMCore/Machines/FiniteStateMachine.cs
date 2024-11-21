using System;
using Game.FSMCore.Profilers;
using Game.FSMCore.States;
using UnityEngine;

namespace Game.FSMCore.Machines
{
public class FiniteStateMachine : IStateMachine, IStateMachineOperator
{
    public IState ActiveState { get; private set; }
    public IState PreviousState { get; private set; }
    public event Action<IState> OnStateChange;
    public StateMachineTree Tree { get; set; } = new();

    void IStateMachine.ChangeState(IState state) => SwitchState(state);

    public void Update()
    {
        ActiveState?.UpdateState();
        Tree.UpdateTree();
    }

    public void StopMachine()
    {
        ActiveState?.Dispose();
        SwitchState(null);
        Tree.DisposeMachine();
        StopDebug();
    }

    [Obsolete("It breaks the concept of the FSM. It will be removed soon")]
    public void ForceTransitTo<TIn>(DeadState<TIn> state, TIn data)
    {
        if (Tree.GetStates().Contains(state) == false)
            throw new ArgumentException("Unknown state");

        ActiveState?.Dispose();
        SwitchState(state);
        Activate(state, data);
    }

    private void SwitchState(IState state)
    {
        PreviousState = ActiveState;
        ActiveState = state;
        OnStateChange?.Invoke(ActiveState);
    }

    private void Activate<TIn>(IActivatedState<TIn> state, TIn data) => state.ActivateState(this, data);
    
    private FSMProfilerProvider _profiler;
    private bool _isOwnProfiler;

    public FiniteStateMachine AddDebugger(GameObject root = null)
    {
        if (Application.isEditor == false)
            return this;
        
        _isOwnProfiler = root == null;
        
        if (root == null)
            root = new GameObject("FSM Linker");
        
        if (root.TryGetComponent(out _profiler))
        {
            _profiler.stateMachine = this;
            return this;
        }

        _profiler = root.AddComponent<FSMProfilerProvider>();
        _profiler.stateMachine = this;
        return this;
    }

    private void StopDebug()
    {
        if (_profiler != null)
            UnityEngine.Object.Destroy(_isOwnProfiler ? _profiler.gameObject : _profiler);
    }
}
}