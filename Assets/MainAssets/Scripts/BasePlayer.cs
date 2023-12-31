using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public enum PlayerState { None = -1, Normal, Fall, Death }

public class BasePlayer : MonoBehaviour, IEnabable, IDisabable
{
    [SerializeField] private GameObject _view;
    [SerializeField] private CharacterController _charController;
    [SerializeField] private BasePlayerController[] _controllers;

    public event Action OnFalling;

    public Vector3 Forward => _transform.forward;
    public Vector3 Position => _transform.position;
    public Quaternion Rotation => _transform.rotation;
    public GameObject View => _view;

    private Transform _transform = null;

    public T GetController<T>() where T : class
    {
        foreach (var controller in _controllers)
        {
            if (controller is T necessaryController)
            { 
                return necessaryController;
            }
        }

        return null;
    }

    public void SetPosition(Vector3 newPosition) => _transform.position = newPosition;
    public void SetRotation(Quaternion newRotation) => _transform.rotation = newRotation;

    private BaseStateMachine _stateMachine = null;

    private void Awake()
    {
        _transform = GetComponent<Transform>();

        _stateMachine = new BaseStateMachine();

        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>
        {
            { typeof(PlayerInitializeState), new PlayerInitializeState(this) },
            { typeof(PlayerIdleState), new PlayerIdleState(this) },
            { typeof(PlayerMovementState), new PlayerMovementState(this) },
            { typeof(PlayerFallState), new PlayerFallState(this) },
            { typeof(PlayerStandUpState), new PlayerStandUpState(this) },
            { typeof(PlayerDeathState), new PlayerDeathState(this) }
        };

        _stateMachine.AddStates(states);
    }

    public virtual void Initialize() => InitializeControllers();
    protected virtual void InitializeControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Initialize(this);
        }
    }

    public void Prepare() => PrepareControllers();
    private void PrepareControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Prepare();
        }
    }

    public void Enable() { }
    public void Disable() { }

    private void Update() => _stateMachine?.Tick();

    #region MovementController

    public void EnableMovement() => _charController.enabled = true;
    public void DisableMovement() => _charController.enabled = false;

    public virtual void Move(Vector3 direction) => _charController.Move(direction);
    public void Rotate(Quaternion rotation) => transform.rotation = rotation;

    #endregion

    public void EnableRagdoll() => OnFalling?.Invoke();

    public void Clear() => ClearControllers();
    private void ClearControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Clear();
        }
    }
}
