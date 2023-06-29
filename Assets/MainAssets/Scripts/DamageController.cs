using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageController : BasePlayerController
{
    [Range(1f, 10f)]
    [SerializeField] private float _mechanicTime = 1f;

    [SerializeField] private BaseLimb[] _limbs;

    private CameraController _cameraController = null;

    private BaseLimb _testLimb = null;

    public override void Initialize(BasePlayer player)
    {
        _cameraController = player.GetController<CameraController>();


    }

    public override void Enable()
    {
        base.Enable();

        _cameraController.OnShiftCompleted += ProcessCameraShiftingDirection;

        SubscribeLimbs();
    }
    private void SubscribeLimbs()
    {
        foreach (var limb in _limbs)
        {
            limb.OnCollided += ProcessLimbsCollision;
        }
    }

    public override void Disable()
    {
        _cameraController.OnShiftCompleted -= ProcessCameraShiftingDirection;

        UnSubscribeLimbs();

        base.Disable();
    }
    private void UnSubscribeLimbs()
    {
        foreach (var limb in _limbs)
        {
            limb.OnCollided -= ProcessLimbsCollision;
        }
    }

    //[Test] Remove
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _testLimb = _limbs[Random.Range(0, _limbs.Length)];

            ShowDamagedLimb();
        }
    }

    private void ProcessLimbsCollision() => ShowDamagedLimb();
    private void ShowDamagedLimb()
    {
        SetTimeScale(0.1f);

        StartCoroutine(ShowLimbRoutine());
    }

    private void SetTimeScale(float value) => Time.timeScale = value;

    private IEnumerator ShowLimbRoutine()
    {
        _cameraController.ShiftLookTo(_testLimb.transform);

        yield return new WaitForSecondsRealtime(_mechanicTime);

        _cameraController.ShiftLookBack();
    }

    //[TODO] Refactoring
    private void ProcessCameraShiftingDirection(ShiftDirectionType direction)
    {
        if (direction == ShiftDirectionType.Backward)
        {
            SetTimeScale(1f);
        }
    }


    public override void Clear()
    {
        _cameraController = null;
    }
}
