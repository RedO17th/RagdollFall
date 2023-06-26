using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    private IMovmentController _movmentController = null;

    public override void Initialize(IPlayer player)
    {
        _movmentController = player.GetController<IMovmentController>();
    }

    public override void Enable()
    {
        base.Enable();

        _movmentController.OnMoveEvent += ProcessMoveState;
        _movmentController.OnStopEvent += ProcessStopState;
    }

    private void ProcessMoveState()
    {
        Debug.Log($"AnimationController.ProcessMoveState");
    }

    private void ProcessStopState()
    {
        Debug.Log($"AnimationController.ProcessStopState");
    }

    public override void Disable()
    {
        _movmentController.OnMoveEvent -= ProcessMoveState;
        _movmentController.OnStopEvent -= ProcessStopState;

        base.Disable();
    }

    public override void Clear()
    {
        
    }
}
