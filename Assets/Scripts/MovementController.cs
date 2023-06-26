using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovmentController
{ 
    
}

public class MovementController : BasePlayerController, IMovmentController
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;

    private IPlayer _player = null;
    private IMovementInput _input = null;

    private Vector3 _normalizedDirection = Vector3.zero;

    private Quaternion rotation = Quaternion.identity;
    private Quaternion targetRotation = Quaternion.identity;

    public override void Initialize(IPlayer player)
    {
        _player = player;

        _input = GetComponent<IMovementInput>();
    }

    public override void Enable()
    {
        _player.EnableMovement();
        
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();

        _player.DisableMovement();
    }

    private void Update()
    {
        _normalizedDirection = _input.GetDirectionByInput();

        ProcessRotation();
        ProcessMove();
    }

    private void ProcessRotation()
    {
        if (_normalizedDirection.magnitude != 0f)
        {
            rotation = Quaternion.LookRotation(_normalizedDirection);
            targetRotation = Quaternion.Slerp(_player.Rotation, rotation, Time.deltaTime * _rotationSpeed);

            _player.Rotate(targetRotation);
        }
    }
    private void ProcessMove() => _player.Move(_normalizedDirection * _movementSpeed * Time.deltaTime);

    public override void Clear()
    {
        _input = null;
        _player = null;

        _normalizedDirection = Vector3.zero;

        rotation = Quaternion.identity;
        targetRotation = Quaternion.identity;
    }
}
