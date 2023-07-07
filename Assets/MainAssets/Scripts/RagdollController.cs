using NTC.Global.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RagdollController : BasePlayerController
{
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

    public event Action OnFell;
    public IReadOnlyList<Transform> Bones => _bones;
    public IReadOnlyList<BoneTransform> RagdollBones => _ragdollBones.ToList();
    public bool IsFaceUp { get; private set; }

    private BasePlayer _player = null;
    private RagdollFallInput _movementInput = null;

    private List<BoneTransform> _ragdollBones;
    private List<Transform> _bones;

    private bool _playerIsFallingFlag = false;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _movementInput = GetComponent<RagdollFallInput>();

        InitializeBonesArrays();
    }

    private void InitializeBonesArrays()
    {
        _bones = _hipsRigidBody.transform.GetComponentsInChildren<Transform>().ToList();

        _ragdollBones = new List<BoneTransform>(_bones.Count);

        for (int i = 0; i < _bones.Count; i++)
        {
            _ragdollBones.Add(new BoneTransform());
        }
    }

    public override void Enable()
    {
        _ragdollOperations.EnableRagdoll();

        base.Enable();
    }

    private void Update()
    {
        CheckFallBegin();
        CheckFallEnd();

        //if (_player.CurrentState == PlayerState.Fall)
        //{
        //    ProcessFallMovement();
        //}
    }

    #region ChackFall

    private void CheckFallBegin()
    {
        if (_playerIsFallingFlag == false && _hipsRigidBody.velocity.magnitude >= _fallBeginTrashold)
        {
            _playerIsFallingFlag = true;
        }
    }

    private void CheckFallEnd()
    {
        if (_playerIsFallingFlag && _hipsRigidBody.velocity.magnitude <= _fallEndTrashold)
        {
            _playerIsFallingFlag = false;

            IsFaceUp = _hipsTransform.forward.y > 0;

            AlignPlayerRotationByHips();
            AlignPlayerPositionByHips();

            SaveBoneTransformInto(_ragdollBones);

            OnFell?.Invoke();
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

        if (IsFaceUp)
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

        if (Physics.Raycast(hipsPosition, Vector3.down, out RaycastHit hitInfo, _rayDistance, _hitMask))
        {
            _player.SetPosition(new Vector3(hipsPosition.x, hitInfo.point.y, hipsPosition.z));
        }

        _hipsTransform.position = hipsPosition;
    }

    private void SaveBoneTransformInto(List<BoneTransform> destination)
    {
        for (int i = 0; i < _bones.Count; i++)
        {
            destination[i].SetPosition(_bones[i].localPosition);
            destination[i].SetRotation(_bones[i].localRotation);
        }
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

    public override void Disable()
    {
        base.Disable();

        _ragdollOperations.DisableRagdoll();
    }

    public override void Clear()
    {
        _playerIsFallingFlag = false;
        IsFaceUp = false;

        _ragdollBones = null;
        _bones = null;

        _player = null;
    }
}
