using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None = -1, Normal, Fall }

public class BasePlayer : MonoBehaviour, IEnabable, IDisabable
{
    [SerializeField] private CharacterController _charController;

    [SerializeField] private BasePlayerController[] _controllers;

    public Quaternion Rotation => _transform.rotation;

    public PlayerState CurrentState { get; private set; } = PlayerState.None;

    private PlayerState _previousState = PlayerState.None;

    private RagdollController _ragdollController = null;
    private Transform _transform = null;

    #region Systematic

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
    public void SetState(PlayerState newState)
    {
        if (newState != CurrentState)
        {
            _previousState = CurrentState;

            CurrentState = newState;
        }
    }
    public void SetPreviousState()
    {
        CurrentState = _previousState;
    }

    #endregion

    //��������
    private void Start()
    {
        Initialize();
        Enable();
    }

    //[TODO] ������� �� ���
    public virtual void Initialize()
    {
        InitializeControllers();

        CurrentState = PlayerState.Normal;
        _previousState = CurrentState;

        _transform = GetComponent<Transform>();

        _ragdollController = GetController<RagdollController>();
    }

    protected virtual void InitializeControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Initialize(this);
        }
    }

    //[TODO] ������� �� ���
    public virtual void Enable()
    {
        _ragdollController.OnFallCompleted += ProcessFallCompletedEvent;

        EnableControllers();
    }

    protected virtual void EnableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Enable();
        }
    }

    //[TODO] ������� �� ���
    public void Disable()
    {
        _ragdollController.OnFallCompleted -= ProcessFallCompletedEvent;

        DisableControllers();
    }
    protected virtual void DisableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Disable();
        }
    }

    #region MovementController

    public void EnableMovement() => _charController.enabled = true;
    public void DisableMovement() => _charController.enabled = false;

    public virtual void Move(Vector3 direction) => _charController.Move(direction);
    public void Rotate(Quaternion rotation) => transform.rotation = rotation;

    #endregion

    #region RagdollController

    public void EnableRagdoll()
    {
        _ragdollController.Unlock();
        _ragdollController.Enable();
    }

    private void ProcessFallCompletedEvent() => DisableRagdoll();
    private void DisableRagdoll()
    {
        _ragdollController.Lock();
        _ragdollController.Disable();

        Debug.Log($"BasePlayer: Fall complition");

        //�������� ��������� ���������, ��� ��� ����������� ��������� ��������.
    }

    #endregion

}
