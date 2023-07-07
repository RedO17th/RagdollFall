using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : BaseState
{
    private MovementController _movementController = null;

    public PlayerMovementState(BasePlayer player) : base(player) 
    {
        _movementController = _player.GetController<MovementController>();
    }

    public override void Enter()
    {
        _movementController.Enable();

        _player.OnFalling += ProcessPlayerFallAction;
    }

    public override void Tick()
    {
        if (Input.GetAxis("Vertical") == 0f && Input.GetAxis("Horizontal") == 0f)
        {
            SendOnChangeEvent<PlayerIdleState>();
        }
    }

    private void ProcessPlayerFallAction()
    {
        SendOnChangeEvent<PlayerFallState>();
    }

    public override void Exit()
    {
        _player.OnFalling -= ProcessPlayerFallAction;

        _movementController.Disable();
    }
}
