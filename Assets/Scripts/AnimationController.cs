using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    private IMovmentController _movmentController = null;

    public override void Initialize(BasePlayer player)
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
        _animator.SetBool("Walking", true);
    }

    private void ProcessStopState()
    {
        _animator.SetBool("Walking", false);
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
