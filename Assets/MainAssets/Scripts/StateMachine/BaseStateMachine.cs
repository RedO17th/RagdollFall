using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine
{
    private Dictionary<Type, BaseState> _states = new Dictionary<Type, BaseState>();
    
    private BaseState _currentState = null;

    public void AddStates(Dictionary<Type, BaseState> states)
    {
        _states = states;

        InitializeFirstState();

        SubscribeStates();
    }

    private void InitializeFirstState()
    {
        if (_currentState == null)
        {
            _currentState = _states[typeof(PlayerInitializeState)];
            _currentState.Enter();
        }
    }

    private void SubscribeStates()
    {
        foreach (var state in _states)
        {
            state.Value.OnChangeEventTo += SetState;
        }
    }
    private void UnSubscribeStates()
    {
        foreach (var state in _states)
        {
            state.Value.OnChangeEventTo -= SetState;
        }
    }

    private void SetState(Type stateType)
    {
        if (_currentState.GetType() == stateType)
        {
            return;
        }

        if (_states.TryGetValue(stateType, out var newState))
        {
            _currentState?.Exit();

            _currentState = newState;

            _currentState.Enter();
        }
    }

    public void Tick() => _currentState?.Tick();
}