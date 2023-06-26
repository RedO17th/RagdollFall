using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public interface IEnabable
{
    void Enable();
}

public interface IDisabable
{
    void Disable();
}

public interface IClearableMemory
{ 
    void Clear();
}

public abstract class BasePlayerController : MonoBehaviour, IEnabable, IDisabable, IClearableMemory
{
    public abstract void Initialize(IPlayer player);

    public virtual void Enable() => enabled = true;
    public virtual void Disable() => enabled = false;

    public abstract void Clear();
}
