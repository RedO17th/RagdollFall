using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLimb : MonoBehaviour
{
    [SerializeField] private int _price = 10;

    public Vector3 Position => transform.position;
    public int Price => _price;



}
