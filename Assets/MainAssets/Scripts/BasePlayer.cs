using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None = -1, Normal, Fall, Death }

public class BasePlayer : MonoBehaviour, IEnabable, IDisabable
{
    [SerializeField] private CharacterController _charController;
    [SerializeField] private BasePlayerController[] _controllers;

    public event Action<bool> OnStandUp;

    public PlayerState CurrentState { get; private set; } = PlayerState.None;
    public Vector3 Forward => transform.forward;
    
    public Vector3 Position => _transform.position;
    public Quaternion Rotation => _transform.rotation;

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

    #endregion

    public void SetState(PlayerState newState)
    {
        if (newState != CurrentState)
        {
            CurrentState = newState;

            ProcessCurrentState();
        }
    }

    public void SetPosition(Vector3 newPosition) => _transform.position = newPosition;
    public void SetRotation(Quaternion newRotation) => _transform.rotation = newRotation;

    //Заглушка
    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

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
    public virtual void Enable() => EnableControllers();
    protected virtual void EnableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Enable();
        }
    }

    public void EnableRagdoll()
    {
        _ragdollController.Unlock();
        _ragdollController.Enable();
    }

    private void ProcessCurrentState()
    {
        switch (CurrentState)
        {
            case PlayerState.Death:
                {
                    DisableRagdoll();
                    ProcessDeathState();
                    break;
                }
        }
    }

    private void DisableRagdoll()
    {
        _ragdollController.Lock();
        _ragdollController.Disable();
    }

    //[TODO] Refactoring
    private void ProcessDeathState() => StartCoroutine(DeathRoutine());
    private IEnumerator DeathRoutine()
    {
        yield return StartCoroutine(_ragdollController.ResetBonesRoutine());

        _animatorController.Enable();

        OnStandUp?.Invoke(_ragdollController.IsFaceUp);
    }

    //[TODO] Вызвать из вне
    public void Disable() => DisableControllers();
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


}

//Заметки:
//Присутствует оффсет, но скорее всего это из-за pivot'a анимации. Transform'ы костей
//тянут меш к точке pivota самой анимации. Оффсет происходит только на анимации FaceUp
