using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _viewGO;

    [SerializeField] private string _faceUpStandUpAName;
    [SerializeField] private string _faceDownStandUpAName;

    [SerializeField] private string _walkingAName;
    [SerializeField] private string _standUpUpTName;
    [SerializeField] private string _standUpDTName;

    private RagdollController _ragdollController = null;
    private IMovmentController _movementController = null;

    private BasePlayer _player = null;

    private BoneTransform[] _standUpFaceUpAnimationBones = null;
    private BoneTransform[] _standUpFaceDownAnimationBones = null;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _ragdollController = _player.GetController<RagdollController>();
        _movementController = _player.GetController<IMovmentController>();
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

    public IReadOnlyList<BoneTransform> GetStandUpAnimationBonesBySide(bool isFaceUp)
    {
        return isFaceUp ? _standUpFaceUpAnimationBones : _standUpFaceDownAnimationBones;
    }

    public override void Prepare()
    {
        RecordAnimationBones();
    }
    private void RecordAnimationBones()
    {
        _standUpFaceUpAnimationBones = new BoneTransform[_ragdollController.Bones.Count];
        _standUpFaceDownAnimationBones = new BoneTransform[_ragdollController.Bones.Count];

        for (int i = 0; i < _ragdollController.Bones.Count; i++)
        {
            _standUpFaceUpAnimationBones[i] = new BoneTransform();
            _standUpFaceDownAnimationBones[i] = new BoneTransform();
        }

        SaveAnimationBonesTransformInto(ReturnAnimationClipByName(_faceUpStandUpAName), _standUpFaceUpAnimationBones);
        SaveAnimationBonesTransformInto(ReturnAnimationClipByName(_faceDownStandUpAName), _standUpFaceDownAnimationBones);
    }
    private void SaveAnimationBonesTransformInto(AnimationClip clip, BoneTransform[] destination)
    {
        var positionBeforeSampling = _player.Position;
        var rotationBeforeSampling = _player.Rotation;

        clip.SampleAnimation(_viewGO, 0f);

        SaveBoneTransformInto(destination);

        _player.SetPosition(positionBeforeSampling);
        _player.SetRotation(rotationBeforeSampling);
    }

    private void SaveBoneTransformInto(BoneTransform[] destination)
    {
        var bones = _ragdollController.Bones;

        for (int i = 0; i < _ragdollController.Bones.Count; i++)
        {
            destination[i].SetPosition(bones[i].localPosition);
            destination[i].SetRotation(bones[i].localRotation);
        }
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

    //[Ref]
    private void ProcessStandUp(bool isUp)
    {
        _animator.SetBool(_walkingAName, false);

        var triggerName = isUp ? _standUpUpTName : _standUpDTName;

        _animator.SetTrigger(triggerName);
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
