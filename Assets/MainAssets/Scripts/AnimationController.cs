using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType { None = -1, Idle, Movement, StandFUp, StandFD }

[Serializable]
public class Animations
{
    [SerializeField] private AnimationType _type;
    [SerializeField] private string _animationName;

    public AnimationType Type => _type;
    public string Name => _animationName;
}

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationEventTransmitter _eventTransmitter;

    [SerializeField] private Animations[] _animations;

    [Space]
    [SerializeField] private string _walkingAName;
    [SerializeField] private string _standUpUpTName;
    [SerializeField] private string _standUpDTName;

    public event Action OnPlayerGotUp;

    private BasePlayer _player = null;

    private RagdollController _ragdollController = null;
    private IMovmentController _movementController = null;

    private BonesSnapshotAnimationHandler _snapshotHandler = null; 

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _ragdollController = _player.GetController<RagdollController>();
        _movementController = _player.GetController<IMovmentController>();
    }

    public override void Prepare()
    {
        _snapshotHandler = new BonesSnapshotAnimationHandler(_ragdollController.Bones);

        CreateSnapshotsByAnimations();
    }

    private void CreateSnapshotsByAnimations()
    {
        foreach (var animation in _animations)
        {
            var positionBeforeSampling = _player.Position; //bugfix
            var rotationBeforeSampling = _player.Rotation;

            var clip = ReturnAnimationClipByName(animation.Name);
                clip.SampleAnimation(_player.View, 0f);

            _snapshotHandler.CreateSnapshot(animation.Type);

            _player.SetPosition(positionBeforeSampling); //bugfix
            _player.SetRotation(rotationBeforeSampling);
        }
    }

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

    public IReadOnlyList<BoneTransform> GetBonesSnapshotBy(AnimationType type)
    {
        return _snapshotHandler.GetSnapshotBy(type);
    }

    public override void Enable()
    {
        base.Enable();

        _animator.enabled = true;

        _eventTransmitter.OnStandUpEnd += ProcessStandUpAnimationEnd;

        _movementController.OnMoveEvent += ProcessMoveState;
        _movementController.OnStopEvent += ProcessStopState;
    }

    public void ProcessStandUp(bool isUp)
    {
        ProcessStopState();

        var triggerName = isUp ? _standUpUpTName : _standUpDTName;

        _animator.SetTrigger(triggerName);
    }

    private void ProcessStandUpAnimationEnd() => OnPlayerGotUp?.Invoke();

    private void ProcessMoveState() => _animator.SetBool(_walkingAName, true);
    private void ProcessStopState() => _animator.SetBool(_walkingAName, false);

    public override void Disable()
    {
        _eventTransmitter.OnStandUpEnd -= ProcessStandUpAnimationEnd;

        _movementController.OnMoveEvent -= ProcessMoveState;
        _movementController.OnStopEvent -= ProcessStopState;

        _animator.enabled = false;

        base.Disable();
    }

    public override void Clear()
    {
        _movementController = null;
        _ragdollController = null;

        _player = null;
    }
}
