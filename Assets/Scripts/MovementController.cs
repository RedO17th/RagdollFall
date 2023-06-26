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

    private Vector3 _normalizedDirection = Vector3.zero;

    private Quaternion rotation = Quaternion.identity;
    private Quaternion targetRotation = Quaternion.identity;

    private float _vertical = 0f;
    private float _horizontal = 0f;

    public override void Initialize(IPlayer player)
    {
        _player = player;
    }

    public override void Enable()
    {
        base.Enable();

        _player.EnableMovement();
    }

    public override void Disable()
    {
        base.Disable();

        _player.DisableMovement();
    }

    private void Update()
    {
        //Обернуть в Input
        _vertical = Input.GetAxis("Vertical");
        _horizontal = Input.GetAxis("Horizontal");
        //..

        //Убрать и передавать в методы
        _normalizedDirection = new Vector3(_horizontal, 0f, _vertical).normalized;

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
        _player = null;

        _normalizedDirection = Vector3.zero;

        rotation = Quaternion.identity;
        targetRotation = Quaternion.identity;

        _vertical = 0f;
        _horizontal = 0f;
    }
}
