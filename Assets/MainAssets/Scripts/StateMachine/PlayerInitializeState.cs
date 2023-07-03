using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitializeState : BaseState
{
    public PlayerInitializeState(BasePlayer player) : base(player)
    {
        
    }

    public override void Enter()
    {
        Debug.Log($"PlayerInitializeState.Enter");

        _player.Initialize();
    }

    public override void Tick() => SendOnChangeEvent<PlayerIdleState>();

    public override void Exit()
    {
        Debug.Log($"PlayerInitializeState.Exit");
    }
}
