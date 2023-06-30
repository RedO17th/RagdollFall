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
    [SerializeField] private GameObject _viewGO;

    [Range(0.05f, 2f)]
    [SerializeField] private float _resetBoneTime = 1f;

    [Range(0.05f, 0.5f)]
    [SerializeField] private float _fallBeginTrashold = 0.2f;
    [Range(0.01f, 0.1f)]
    [SerializeField] private float _fallEndTrashold = 0.03f;

    [SerializeField] private Transform _hipsTransform;
    [SerializeField] private Rigidbody _hipsRigidBody;
    [SerializeField] private RagdollOperations _ragdollOperations;

    [Range(10, 700)]
    [SerializeField] private float _movementForce = 300f;

    [Range(1f, 21f)]
    [SerializeField] private float _maxMovementVelocity = 5f;

    [Range(5, 100)]
    [SerializeField] private float _angularForce = 10f;

    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _rayDistance = 1f;

    public bool IsFaceUp => _isFaceUp;

    private RagdollFallInput _movementInput = null;

    private MovementController _movementController = null;
    private AnimationController _animatorController = null;

    private BasePlayer _player = null;

    private BoneTransform[] _standUpFaceUpAnimationBones;
    private BoneTransform[] _standUpFaceDownAnimationBones;
    private BoneTransform[] _ragdollBones;
    private Transform[] _bones;

    private bool _playerIsFalling = false;
    private bool _isFaceUp = false;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _movementInput = GetComponent<RagdollFallInput>();

        _movementController = _player.GetController<MovementController>();
        _animatorController = _player.GetController<AnimationController>();

        InitializeBonesArrays();

        SaveAnimationBonesTransformInto(_animatorController.ReturnStandUPFaceUPClip(), _standUpFaceUpAnimationBones);
        SaveAnimationBonesTransformInto(_animatorController.ReturnStandUPDownClip(), _standUpFaceDownAnimationBones);
    }

    private void InitializeBonesArrays()
    {
        _bones = _hipsRigidBody.transform.GetComponentsInChildren<Transform>();

        _standUpFaceUpAnimationBones = new BoneTransform[_bones.Length];
        _standUpFaceDownAnimationBones = new BoneTransform[_bones.Length];

        _ragdollBones = new BoneTransform[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            _standUpFaceUpAnimationBones[i] = new BoneTransform();
            _standUpFaceDownAnimationBones[i] = new BoneTransform();
            
            _ragdollBones[i] = new BoneTransform();
        }
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

            _ragdollOperations.EnableRagdoll();

            base.Enable();
        }
    }

    private void Update()
    {
        CheckFallComplition();

        if (_player.CurrentState == PlayerState.Fall)
        {
            ProcessFallMovement();
        }
    }

    #region ChackFall

    private void CheckFallComplition()
    {
        CheckFallBegin();

        CheckFallEnd();
    }

    private void CheckFallBegin()
    {
        if (_playerIsFalling == false && _hipsRigidBody.velocity.magnitude >= _fallBeginTrashold)
        {
            _playerIsFalling = true;

            _player.SetState(PlayerState.Fall);
        }
    }

    private void CheckFallEnd()
    {
        if (_playerIsFalling && _hipsRigidBody.velocity.magnitude <= _fallEndTrashold)
        {
            _playerIsFalling = false;

            _isFaceUp = _hipsTransform.forward.y > 0;

            AlignPlayerRotationByHips();
            AlignPlayerPositionByHips();

            SaveBoneTransformInto(_ragdollBones);

            _player.SetState(PlayerState.Death);
        }
    }

    private void AlignPlayerRotationByHips()
    { 
        var hipsPosition = _hipsTransform.position;
        var hipsRotation = _hipsTransform.rotation;

        _player.SetRotation(GetRotationToHips());

        _hipsTransform.position = hipsPosition;
        _hipsTransform.rotation = hipsRotation;
    }
    private Quaternion GetRotationToHips()
    {
        var desiredPosition = _hipsTransform.up;

        if (_isFaceUp)
        {
            desiredPosition *= -1;
        }

        desiredPosition.y = 0;
        desiredPosition.Normalize();

        return _player.Rotation * Quaternion.FromToRotation(_player.Forward, desiredPosition);
    }

    private void AlignPlayerPositionByHips()
    {
        var hipsPosition = _hipsTransform.position;

        #region Old

        //var posOffset = GetBoneTransforms()[0].Position;
        //    posOffset.y = 0;
        //    posOffset = _player.Rotation * posOffset;

        //_player.SetPosition(_player.Position - posOffset);

        #endregion

        if (Physics.Raycast(hipsPosition, Vector3.down, out RaycastHit hitInfo, _rayDistance, _hitMask))
        {
            //_player.SetPosition(new Vector3(_player.Position.x, hitInfo.point.y, _player.Position.z));
            
            _player.SetPosition(new Vector3(hipsPosition.x, hitInfo.point.y, hipsPosition.z));
        }

        _hipsTransform.position = hipsPosition;
    }

    private BoneTransform[] GetBoneTransforms()
    {
        return _isFaceUp ? _standUpFaceUpAnimationBones : _standUpFaceDownAnimationBones;
    }

    #endregion

    #region FallMovement

    private void ProcessFallMovement()
    {
        var movementDirection = _movementInput.GetMovementDirection();
        var rotateDirection = _movementInput.GetRotateDirection();

        var movementVector = new Vector3(movementDirection, 0f, 0f).normalized;
        var torqueVector = new Vector3(0f, rotateDirection, 0f).normalized;

        _hipsRigidBody.AddForce(movementVector * _movementForce, ForceMode.Force);
        _hipsRigidBody.AddTorque(torqueVector * _angularForce, ForceMode.Force);            

        ClampPlayerVelocityMagnitude();
    }

    private void ClampPlayerVelocityMagnitude()
    {
        if (_hipsRigidBody.velocity.magnitude > _maxMovementVelocity)
        {
            _hipsRigidBody.velocity = Vector3.ClampMagnitude(_hipsRigidBody.velocity, _maxMovementVelocity);
        }
    }

    #endregion

    public IEnumerator ResetBonesRoutine()
    {
        var percent = 0f;
        var elapsedTime = 0f;

        var targetBoneTransforms = GetBoneTransforms();

        while (elapsedTime < _resetBoneTime)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                percent = elapsedTime / _resetBoneTime;

                _bones[i].localPosition = Vector3.Lerp(_ragdollBones[i].Position, targetBoneTransforms[i].Position, percent);
                _bones[i].localRotation = Quaternion.Lerp(_ragdollBones[i].Rotation, targetBoneTransforms[i].Rotation, percent);
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
        _playerIsFalling = false;
        _isFaceUp = false;

        _movementController = null;
        _animatorController = null;

        _standUpFaceUpAnimationBones = null;
        _standUpFaceDownAnimationBones = null;

        _ragdollBones = null;
        _bones = null;

        _player = null;
    }
}
