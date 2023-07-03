using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovmentController
{
    public event Action OnMoveEvent;   
    public event Action OnStopEvent;   
}

public class MovementController : BasePlayerController, IMovmentController
{
    [Range(1f, 10f)]
    [SerializeField] private float _movementSpeed = 5f;

    [Range(1f, 30f)]
    [SerializeField] private float _rotationSpeed = 5f;

    public event Action OnMoveEvent;
    public event Action OnStopEvent;

    private BasePlayer _player = null;
    private IMovementInput _input = null;

    private Vector3 _normalizedDirection = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private Quaternion _targetRotation = Quaternion.identity;

    private bool _motionStateSwitchFlag = false;

    public override void Initialize(BasePlayer player)
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

        ForceStopIfNecessary();
    }

    private void ForceStopIfNecessary()
    {
        if (_motionStateSwitchFlag)
        {
            _motionStateSwitchFlag = false;

            OnStopEvent?.Invoke();
        }
    }

    private void Update()
    {
        _normalizedDirection = _input.GetDirectionByInput();

        ProcessMotionStateEvents();

        ProcessRotation();
        ProcessMove();
    }

    private void ProcessMotionStateEvents()
    {
        if (_normalizedDirection != Vector3.zero)
        {
            if (_motionStateSwitchFlag == false)
            {
                _motionStateSwitchFlag = true;

                OnMoveEvent?.Invoke();
            }
        }
        else
        {
            if (_motionStateSwitchFlag)
            {
                _motionStateSwitchFlag = false;

                OnStopEvent?.Invoke();
            }
        }
    }

    private void ProcessRotation()
    {
        if (_normalizedDirection.magnitude != 0f)
        {
            _rotation = Quaternion.LookRotation(_normalizedDirection);
            _targetRotation = Quaternion.Slerp(_player.Rotation, _rotation, Time.deltaTime * _rotationSpeed);

            _player.Rotate(_targetRotation);
        }
    }
    private void ProcessMove() => _player.Move(_normalizedDirection * _movementSpeed * Time.deltaTime);

    public override void Clear()
    {
        _input = null;
        _player = null;

        _normalizedDirection = Vector3.zero;

        _rotation = Quaternion.identity;
        _targetRotation = Quaternion.identity;
    }
}
