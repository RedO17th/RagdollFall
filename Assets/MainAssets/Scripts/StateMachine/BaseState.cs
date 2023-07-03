using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public event Action<Type> OnChangeEventTo;

    protected BasePlayer _player = null;

    public BaseState(BasePlayer player)
    {
        _player = player;
    }

    protected void SendOnChangeEvent<T>() => OnChangeEventTo?.Invoke(typeof(T));

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}
