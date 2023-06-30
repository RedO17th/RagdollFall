using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollFallInput : MonoBehaviour
{
    public float GetMovementDirection()
    { 
        return Input.GetAxis("Horizontal") * -1f;
    }

    public float GetRotateDirection()
    {
        return Input.GetAxis("Vertical") * -1f;
    }
}
