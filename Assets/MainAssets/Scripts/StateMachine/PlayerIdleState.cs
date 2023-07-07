using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : BaseState
{
    private CameraController _cameraController = null;
    private AnimationController _animationController = null;

    public PlayerIdleState(BasePlayer player) : base(player)
    {
        _cameraController = _player.GetController<CameraController>();
        _animationController = _player.GetController<AnimationController>();
    }

    public override void Enter()
    {
        _cameraController.Enable();
        _animationController.Enable();
    }

    //[Ref]
    public override void Tick()
    {
        if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
        {
            SendOnChangeEvent<PlayerMovementState>();
        }
    }

    public override void Exit() { }
}
