using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public interface IMovable
{
    void EnableMovement();
    void DisableMovement();

    void Move(Vector3 direction);
}

public interface IRotatable
{
    Quaternion Rotation { get; }
    void Rotate(Quaternion rotation);
}

public interface IPlayer : IMovable, IRotatable, IEnabable, IDisabable
{
    T GetController<T>() where T : class;

    void Initialize();
}

public class BasePlayer : MonoBehaviour, IPlayer
{
    [SerializeField] private CharacterController _charController;

    [SerializeField] private BasePlayerController[] _controllers;

    public Quaternion Rotation => _transform.rotation;

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

    //Заглушка
    private void Start()
    {
        Initialize();
        Enable();
    }

    //[TODO] Вызвать из вне
    public virtual void Initialize()
    {
        _transform = GetComponent<Transform>();

        InitializeControllers();
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

    //[TODO] Вызвать из вне
    public void Disable() => DisableControllers();
    protected virtual void DisableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Disable();
        }
    }

    public void EnableMovement() => _charController.enabled = true;
    public void DisableMovement() => _charController.enabled = false;

    public virtual void Move(Vector3 direction) => _charController.Move(direction);
    public void Rotate(Quaternion rotation) => transform.rotation = rotation;
}
