using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None = -1, Normal, Fall, Death }

public class BasePlayer : MonoBehaviour, IEnabable, IDisabable
{
    [SerializeField] private CharacterController _charController;

    [SerializeField] private BasePlayerController[] _controllers;

    [SerializeField] private Transform _hipsTransform;
    
    public event Action OnStandUp;

    public Quaternion Rotation => _transform.rotation;

    public PlayerState CurrentState { get; private set; } = PlayerState.None;

    private RagdollController _ragdollController = null;
    private AnimationController _animatorController = null;

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
            CurrentState = newState;

            ProcessCurrentState();
        }
    }

    #endregion

    //Заглушка
    private void Start()
    {
        Initialize();
        Enable();
    }

    //[TODO] Вызвать из вне
    public virtual void Initialize()
    {
        InitializeControllers();

        CurrentState = PlayerState.Normal;
        //_previousState = CurrentState;

        _transform = GetComponent<Transform>();

        _ragdollController = GetController<RagdollController>();
        _animatorController = GetController<AnimationController>();
    }

    protected virtual void InitializeControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Initialize(this);
        }
    }

    //[TODO] Вызвать из вне
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

    //[TODO] Вызвать из вне
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

    private void ProcessFallCompletedEvent()
    {
        Debug.Log($"BasePlayer.ProcessFallCompletedEvent");

        DisableRagdoll();
        SetState(PlayerState.Death);
    }

    private void DisableRagdoll()
    {
        _ragdollController.Lock();
        _ragdollController.Disable();
    }

    #endregion

    private void ProcessCurrentState()
    {
        switch (CurrentState)
        {
            case PlayerState.Death:
                {
                    ProcessDeathState();
                    break;
                }
        }
    }

    //[TODO] Refactoring
    private void ProcessDeathState()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    { 
        yield return StartCoroutine(_ragdollController.ResetBonesRoutine());

        _animatorController.Enable();

        OnStandUp?.Invoke();
    }
}
