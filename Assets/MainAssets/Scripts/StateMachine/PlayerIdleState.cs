using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : BaseState
{
    private AnimationController _animationController = null;

    public PlayerIdleState(BasePlayer player) : base(player)
    {
        _animationController = _player.GetController<AnimationController>();
    }

    public override void Enter()
    {
        Debug.Log($"PlayerIdleState.Enter");

        _animationController.Enable();
    }

    public override void Tick()
    {
        if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
        {
            SendOnChangeEvent<PlayerMovementState>();
        }
    }

    public override void Exit()
    {
        Debug.Log($"PlayerIdleState.Exit");
    }
}
