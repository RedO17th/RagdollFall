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
        if (_hipsRigidBody.velocity.magnitude <= 0f)
        {
            OnFallCompleted?.Invoke();
        }
    }

    public override void Disable()
    {
        base.Disable();

        _ragdollOperations.DisableRagdoll();

        //..
        //_animatorController.Enable();
        //_movementController.Enable();

        _player.SetPreviousState();
    }

    public override void Clear()
    {
        _movementController = null;
        _animatorController = null;

        _player = null;
    }
}
