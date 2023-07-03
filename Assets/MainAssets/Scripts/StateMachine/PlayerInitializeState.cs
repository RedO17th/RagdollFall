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
        _player.Initialize();
        _player.Prepare();
    }

    public override void Tick() => SendOnChangeEvent<PlayerIdleState>();

    public override void Exit() { }
}
