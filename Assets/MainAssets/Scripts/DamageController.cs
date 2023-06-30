using System.Collections;
using UnityEngine;

public class DamageController : BasePlayerController
{
    [Range(1f, 10f)]
    [SerializeField] private float _mechanicTime = 1f;

    [SerializeField] private BaseLimb[] _limbs;

    private BasePlayer _player = null;

    private CameraController _cameraController = null;

    private BaseLimb _currentDamagedLimb = null;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _cameraController = _player.GetController<CameraController>();

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

        _player.OnStateChanged += ProcessPlayerStates;

        _cameraController.OnShiftCompleted += ProcessCameraShiftingDirection;
    }

    public override void Disable()
    {
        _player.OnStateChanged -= ProcessPlayerStates;

        _cameraController.OnShiftCompleted -= ProcessCameraShiftingDirection;

        base.Disable();
    }

    private void ProcessPlayerStates(PlayerState state)
    {
        if (state == PlayerState.Fall)
        {
            ProcessPlayerFallState();
        }

        if (state == PlayerState.Death)
        {
            ProcessPlayerDeathState();
        }
    }

    private void ProcessPlayerFallState() => EnableLimbs();
    private void ProcessPlayerDeathState() => DisableLimbs();

    private void ProcessLimbsCollision(BaseLimb damagedLimb)
    {
        _currentDamagedLimb = damagedLimb;

        Debug.Log($"DamageController: Limb {_currentDamagedLimb.gameObject.name}, price = {_currentDamagedLimb.Price} ");

        DisableLimbs();

        ShowDamagedLimb();
    }
    private void DisableLimbs()
    {
        foreach (var limb in _limbs)
        {
            limb.OnCollided -= ProcessLimbsCollision;
            limb.Disable();
        }
    }

    private void ShowDamagedLimb()
    {
        SetTimeScale(0.1f);

        StartCoroutine(ShowLimbRoutine());
    }

    private void SetTimeScale(float value) => Time.timeScale = value;

    private IEnumerator ShowLimbRoutine()
    {
        _cameraController.ShiftLookTo(_currentDamagedLimb.transform);

        yield return new WaitForSecondsRealtime(_mechanicTime);

        _cameraController.ShiftLookBack();

        EnableLimbs();
    }

    private void EnableLimbs()
    {
        foreach (var limb in _limbs)
        {
            limb.Enable();
            limb.OnCollided += ProcessLimbsCollision;
        }
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
        _currentDamagedLimb = null;
        _cameraController = null;
        _player = null;
    }
}
