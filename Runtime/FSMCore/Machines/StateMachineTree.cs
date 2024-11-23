using System;
using System.Collections.Generic;
using System.Linq;
using Game.FSMCore.States;
using Game.FSMCore.Transitions;
using Game;

namespace Game.FSMCore.Machines
{
public class StateMachineTree
{
    private List<TransitionData> _transitions = new(5);
    private readonly List<IState> _states = new();

    internal void UpdateTree()
    {
        foreach (var transitionData in _transitions)
        {
            if (transitionData.transition.Decide() == false)
                continue;

            transitionData.transition.Transit();

            return;
        }
    }

    internal void DisposeMachine()
    {
        foreach (var transitionData in _transitions)
        {
            transitionData.transition.Dispose();
        }
    }

    #region States

    public StateMachineTree AddState(IState state)
    {
        _states.Add(state);

        return this;
    }

    public StateMachineTree AddState(params IState[] states)
    {
        foreach (var state in states)
        {
            AddState(state);
        }

        return this;
    }

    public StateMachineTree RemoveState(IState state)
    {
        if (_states.Remove(state) == false)
        {
            Log.Warning($"Can't remove state from tree: StateType={state.GetType()}");
        }

        return this;
    }

    public List<IState> GetStates() => _states;

    #endregion

    #region Transitions

    public StateMachineTree AddTransition(BaseTransition baseTransition, int priority = 0)
    {
        _transitions.Add(new TransitionData
        {
            transition = baseTransition,
            priority = priority,
        });

        _transitions = _transitions.OrderByDescending(x => x.priority).ToList();

        return this;
    }

    public StateMachineTree AddTransition(params BaseTransition[] baseTransition)
    {
        foreach (var transition in baseTransition)
        {
            _transitions.Add(new TransitionData
            {
                transition = transition,
                priority = 0,
            });
        }

        _transitions = _transitions.OrderByDescending(x => x.priority).ToList();

        return this;
    }

    public StateMachineTree RemoveTransition(BaseTransition baseTransition)
    {
        var transitionData = _transitions.FirstOrDefault(x => x.transition == baseTransition);
        if (transitionData.Equals(default) || transitionData.transition == null)
        {
            Log.Warning($"Can't remove transition from tree: Transition={baseTransition}");

            return this;
        }

        _transitions.Remove(transitionData);

        return this;
    }

    public TTransition GetTransition<TTransition>() where TTransition : BaseTransition
    {
        foreach (var transitionData in _transitions)
        {
            if (transitionData.transition.GetType() == typeof(TTransition))
            {
                return transitionData.transition as TTransition;
            }
        }

        return default;
    }

    public IEnumerable<BaseTransition> GetTransitions() => _transitions.Select(x => x.transition);

    #endregion

    public IState this[int i] => _states[i];
    
    public IState this[Type type] => _states.FirstOrDefault(x => x.GetType() == type);
}

internal struct TransitionData
{
    public BaseTransition transition;
    public int priority;
}
}