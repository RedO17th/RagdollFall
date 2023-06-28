using NTC.Global.System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RagdollController : BasePlayerController
{
    [SerializeField] private bool _isLocked = true;

    [SerializeField] private Rigidbody _hipsRigidBody;
    [SerializeField] private RagdollOperations _ragdollOperations;

    public event Action OnFallCompleted;

    private MovementController _movementController = null;
    private AnimationController _animatorController = null;

    private BasePlayer _player = null;

    private bool _playerIsFalling = false;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _movementController = _player.GetController<MovementController>();
        _animatorController = _player.GetController<AnimationController>();
    }

    public void Unlock() => _isLocked = false;
    public void Lock() => _isLocked = true;

    public override void Enable()
    {
        if (_isLocked == false)
        {
            _player.SetState(PlayerState.Fall);

            _movementController.Disable();
            _animatorController.Disable();

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
        if (_playerIsFalling == false && _hipsRigidBody.velocity.magnitude >= 0.1f)
        {
            _playerIsFalling = true;

            Debug.Log($"RagdollController.CheckFallComplition: ����� ������");
        }
    }

    private void CheckFallEnd()
    {
        if (_playerIsFalling && _hipsRigidBody.velocity.magnitude <= 0.05f)
        {
            Debug.Log($"RagdollController.CheckFallComplition: ����");

            _playerIsFalling = false;

            OnFallCompleted?.Invoke();
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

        _player = null;
    }
}