using System;
using System.Collections;
using UnityEngine;

public class DamageController : BasePlayerController
{
    [SerializeField] private BaseLimb[] _limbs;

    public event Action<BaseLimb> OnDamage;

    public override void Initialize(BasePlayer player)
    {
        LimbsInitialize();
    }

    private void LimbsInitialize()
    {
        foreach (var limb in _limbs)
        {
            limb.Initialize();
        }
    }

    public override void Enable()
    {
        base.Enable();

        EnableLimbs();
    }

    public override void Disable()
    {
        DisableLimbs();

        base.Disable();
    }

    private void EnableLimbs()
    {
        foreach (var limb in _limbs)
        {
            limb.Enable();
            limb.OnCollided += ProcessLimbsCollision;
        }
    }

    private void ProcessLimbsCollision(BaseLimb damagedLimb) => OnDamage?.Invoke(damagedLimb);
    private void DisableLimbs()
    {
        foreach (var limb in _limbs)
        {
            limb.OnCollided -= ProcessLimbsCollision;
            limb.Disable();
        }
    }

    public override void Clear() { }
}
