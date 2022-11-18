using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

public class StateMachine
{
    public IState CurrentState;

    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();

    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    private float time = 0f;

    public void Tick(float deltaTime)
    {
        var transition = GetTransition();
        if (transition != null)
        {
            if (time > transition.ReactionTimeRange.y)
            {
                SetState(transition.To);
                time = 0f;
            }
            time += deltaTime;
        }

        CurrentState?.Tick();
    }

    public void SetState(IState state)
    {
        if (state == CurrentState)
            return;

        CurrentState?.OnExit();
        CurrentState = state;

        _transitions.TryGetValue(CurrentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        CurrentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate, Vector2 reactionTimeRange)
    {
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }

        transitions.Add(new Transition(to, predicate, reactionTimeRange));
    }

    public void AddAnyTransition(IState state, Func<bool> predicate, Vector2 reactionTimeRange)
    {
        _anyTransitions.Add(new Transition(state, predicate, reactionTimeRange));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }
        public Vector2 ReactionTimeRange { get; }

        public Transition(IState to, Func<bool> condition, Vector2 reactionTimeRange)
        {
            To = to;
            Condition = condition;
            ReactionTimeRange = reactionTimeRange;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
}