using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    [SerializeField] private string _faceUpStandUpAName;
    [SerializeField] private string _faceDownStandUpAName;

    [SerializeField] private string _walkingAName;
    [SerializeField] private string _standUpUpTName;
    [SerializeField] private string _standUpDTName;

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

    private void ProcessMoveState() => _animator.SetBool(_walkingAName, true);
    private void ProcessStopState() => _animator.SetBool(_walkingAName, false);

    private void ProcessStandUp(bool isUp)
    {
        _animator.SetBool(_walkingAName, false);

        var triggerName = isUp ? _standUpUpTName : _standUpDTName;

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

    //..

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
