using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementInput
{
    Vector3 GetDirectionByInput();
}

public class MovementInput : MonoBehaviour, IMovementInput
{
    private float _vertical = 0f;
    private float _horizontal = 0f;

    public Vector3 GetDirectionByInput()
    {
        _vertical = Input.GetAxis("Vertical");
        _horizontal = Input.GetAxis("Horizontal");

        return new Vector3(_horizontal, 0f, _vertical).normalized;
    }
}
