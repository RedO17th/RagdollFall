using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLimb : MonoBehaviour, IEnabable, IDisabable
{
    [SerializeField] private float _damageTrashold = 1f;

    [Space]
    [SerializeField] private int _price = 10;

    public event Action<BaseLimb> OnCollided;

    public Vector3 Position => transform.position;
    public int Price => _price;

    private Rigidbody _rigidBodyLimb = null;

    private void Start() { }

    public void Initialize()
    {
        _rigidBodyLimb = GetComponent<Rigidbody>();
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsObstacle(collision) && CanDamaged())
        {
            ProcessDamage();
        }
    }

    private bool IsObstacle(Collision collision)
    {
        var obstacle = collision.rigidbody?.gameObject.GetComponent<BaseObstacle>();

        return (obstacle != null) ? true : false;
    }

    private bool CanDamaged()
    { 
        return _rigidBodyLimb.velocity.magnitude >= _damageTrashold;
    }

    private void ProcessDamage()
    {
        OnCollided?.Invoke(this);
    }


}