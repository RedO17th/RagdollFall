using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    [SerializeField] private string _faceUpStandUpAName;
    [SerializeField] private string _faceDownStandUpAName;

    private IMovmentController _movementController = null;

    private BasePlayer _player = null;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _movementController = _player.GetController<IMovmentController>();
    }

    public override void Enable()
    {
        base.Enable();

        _animator.enabled = true;

        _player.OnStandUp += ProcessStandUp;

        _movementController.OnMoveEvent += ProcessMoveState;
        _movementController.OnStopEvent += ProcessStopState;
    }

    private void ProcessMoveState() => _animator.SetBool("Walking", true);
    private void ProcessStopState() => _animator.SetBool("Walking", false);

    private void ProcessStandUp(bool isUp)
    {
        _animator.SetBool("Walking", false);

        var triggerName = isUp ? "StandUpFaceUp" : "StandUpFaceDown";

        _animator.SetTrigger(triggerName);
    }

    //[TODO] Ref?
    public AnimationClip ReturnStandUPFaceUPClip() => ReturnAnimationClipByName(_faceUpStandUpAName);
    public AnimationClip ReturnStandUPDownClip() => ReturnAnimationClipByName(_faceDownStandUpAName);

    private AnimationClip ReturnAnimationClipByName(string name)
    {
        AnimationClip result = null;

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                result = clip;
                break;
            }
        }

        return result;
    }

    public override void Disable()
    {
        _player.OnStandUp -= ProcessStandUp;

        _movementController.OnMoveEvent -= ProcessMoveState;
        _movementController.OnStopEvent -= ProcessStopState;

        _animator.enabled = false;

        base.Disable();
    }

    public override void Clear()
    {
        
    }
}
