using NTC.Global.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollController : BasePlayerController
{

    [Space]
    [SerializeField] private bool _isLocked = true;

    [Range(0.05f, 2f)]
    [SerializeField] private float _resetBoneTime = 1f;

    [SerializeField] private GameObject _viewGO;

    [SerializeField] private Rigidbody _hipsRigidBody;
    [SerializeField] private RagdollOperations _ragdollOperations;

    private MovementController _movementController = null;
    private AnimationController _animatorController = null;

    private BasePlayer _player = null;

    private BoneTransform[] _standUpAnimationBones;
    private BoneTransform[] _ragdollBones;
    private Transform[] _bones;

    private bool _playerIsFalling = false;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _movementController = _player.GetController<MovementController>();
        _animatorController = _player.GetController<AnimationController>();

        InitializeBonesArrays();
        SaveAnimationBonesTransformInto(_standUpAnimationBones);
    }

    private void InitializeBonesArrays()
    {
        _bones = _hipsRigidBody.transform.GetComponentsInChildren<Transform>();

        _standUpAnimationBones = new BoneTransform[_bones.Length];
        _ragdollBones = new BoneTransform[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            _standUpAnimationBones[i] = new BoneTransform();
            _ragdollBones[i] = new BoneTransform();
        }
    }

    private void SaveAnimationBonesTransformInto(BoneTransform[] destination)
    {
        var positionBeforeSampling = _player.Position;
        var rotationBeforeSampling = _player.Rotation;

        var clip = _animatorController.ReturnStandUPClip();
            clip.SampleAnimation(_viewGO, 0f);

        SaveBoneTransformInto(destination);

        _player.SetPosition(positionBeforeSampling);
        _player.SetRotation(rotationBeforeSampling);
    }

    private void SaveBoneTransformInto(BoneTransform[] destination)
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            destination[i].SetPosition(_bones[i].localPosition);
            destination[i].SetRotation(_bones[i].localRotation);
        }
    }

    public void Unlock() => _isLocked = false;
    public void Lock() => _isLocked = true;

    public override void Enable()
    {
        if (_isLocked == false)
        {
            _movementController.Disable();
            _animatorController.Disable();

            _player.SetState(PlayerState.Fall);

            _ragdollOperations.EnableRagdoll();

            base.Enable();
        }
    }

    private void Update() => CheckFallComplition();
    private void CheckFallComplition()
    {
        CheckFallBegin();

        CheckFallEnd();
    }

    private void CheckFallBegin()
    {
        if (_playerIsFalling == false && _hipsRigidBody.velocity.magnitude >= 0.2f)
        {
            _playerIsFalling = true;
        }
    }

    private void CheckFallEnd()
    {
        if (_playerIsFalling && _hipsRigidBody.velocity.magnitude <= 0.05f)
        {
            _playerIsFalling = false;

            AlignPlayerPivotByHips();
            SaveBoneTransformInto(_ragdollBones);

            _player.SetState(PlayerState.Death);
        }
    }

    private void AlignPlayerPivotByHips()
    {
        var hipsPosition = _hipsRigidBody.transform.position;

        if (Physics.Raycast(hipsPosition, Vector3.down, out RaycastHit hitInfo))
        {
            _player.SetPosition(new Vector3(hipsPosition.x, hitInfo.point.y, hipsPosition.z));
        }

        _hipsRigidBody.transform.position = hipsPosition;
    }

    public IEnumerator ResetBonesRoutine()
    {
        var percent = 0f;
        var elapsedTime = 0f;

        while (elapsedTime < _resetBoneTime)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                percent = elapsedTime / _resetBoneTime;

                _bones[i].localPosition = Vector3.Lerp(_ragdollBones[i].Position, _standUpAnimationBones[i].Position, percent);
                _bones[i].localRotation = Quaternion.Lerp(_ragdollBones[i].Rotation, _standUpAnimationBones[i].Rotation, percent);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    public override void Disable()
    {
        base.Disable();

        _ragdollOperations.DisableRagdoll();
    }

    public override void Clear()
    {
        _movementController = null;
        _animatorController = null;

        _standUpAnimationBones = null;
        _ragdollBones = null;
        _bones = null;

        _player = null;
    }
}
