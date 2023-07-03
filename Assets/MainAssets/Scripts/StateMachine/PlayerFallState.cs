using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : BaseState
{
    private RagdollController _ragdollController = null;

    private MovementController _movementController = null;
    private AnimationController _animatorController = null;

    public PlayerFallState(BasePlayer player) : base(player)
    {
        _ragdollController = _player.GetController<RagdollController>();

        _movementController = _player.GetController<MovementController>();
        _animatorController = _player.GetController<AnimationController>();
    }

    public override void Enter()
    {
        Debug.Log($"PlayerFallState.Enter");

        _movementController.Disable();
        _animatorController.Disable();

        _ragdollController.OnFell += ProcessPlayerFell;
        _ragdollController.Enable();
    }

    private void ProcessPlayerFell()
    {
        Debug.Log($"PlayerFallState.ProcessPlayerFell");

        SendOnChangeEvent<PlayerStandUpState>();
    }

    public override void Tick()
    {
        
    }

    public override void Exit()
    {
        Debug.Log($"PlayerFallState.Exit");

        _ragdollController.OnFell -= ProcessPlayerFell;

        _ragdollController.Disable();
    }
}
