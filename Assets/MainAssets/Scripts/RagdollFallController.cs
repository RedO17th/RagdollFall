using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollFallController : BasePlayerController
{
    [Header("Controlled body")]
    [SerializeField] private Rigidbody _hipsRigidBody;

    [Header("Settings")]
    [Range(10, 700)]
    [SerializeField] private float _movementForce = 300f;

    [Range(1f, 21f)]
    [SerializeField] private float _maxMovementVelocity = 5f;

    [Range(5, 100)]
    [SerializeField] private float _angularForce = 10f;

    private RagdollFallInput _movementInput = null;

    public override void Initialize(BasePlayer player)
    {
        _movementInput = GetComponent<RagdollFallInput>();
    }

    private void Update() => ProcessFallMovement();

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

    public override void Clear()
    {
        _movementInput = null;
    }
}
