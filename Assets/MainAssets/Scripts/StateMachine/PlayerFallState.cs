using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : BaseState
{
    private const float MECHANICTIME = 2f;
    private const float MINTIMESCALE = 0.3f;
    private const float MAXTIMESCALE = 1f;

    private RagdollController _ragdollController = null;
    private RagdollFallController _ragdollFallController = null;

    private DamageController _damageController = null;
    private CameraController _cameraController = null;
    private MovementController _movementController = null;
    private AnimationController _animatorController = null;

    public PlayerFallState(BasePlayer player) : base(player)
    {
        _ragdollController = _player.GetController<RagdollController>();
        _ragdollFallController = _player.GetController<RagdollFallController>();

        _damageController = _player.GetController<DamageController>();
        _cameraController = _player.GetController<CameraController>();

        _movementController = _player.GetController<MovementController>();
        _animatorController = _player.GetController<AnimationController>();
    }

    public override void Enter()
    {
        _movementController.Disable();
        _animatorController.Disable();

        _cameraController.OnShiftCompleted += ProcessCameraShifting;

        _damageController.OnDamage += ProcessLimbDamage;
        _damageController.Enable();

        _ragdollController.OnFell += ProcessPlayerFell;
        _ragdollController.Enable();

        _ragdollFallController.Enable();
    }

    private void ProcessLimbDamage(BaseLimb limb)
    {
        _damageController.Disable();

        SetTimeScale(MINTIMESCALE);
        StartCameraZoomMechanic(limb);
    }

    private void SetTimeScale(float value) => Time.timeScale = value;
    private void StartCameraZoomMechanic(BaseLimb limb) => _cameraController.ShiftLookTo(limb.transform);

    private void ProcessCameraShifting(ShiftDirectionType direction)
    {
        if (direction == ShiftDirectionType.Max)
        {
            _player.StartCoroutine(TimerRoutine());
        }
        else
        {
            if (direction == ShiftDirectionType.Min)
            {
                SetTimeScale(MAXTIMESCALE);

                _damageController.Enable();
            }
        }
    }

    private IEnumerator TimerRoutine()
    {
        yield return new WaitForSecondsRealtime(MECHANICTIME);

        StopCameraZoomMechanic();
    }

    private void StopCameraZoomMechanic() => _cameraController.ShiftLookBack();

    private void ProcessPlayerFell()
    {
        _ragdollController.OnFell -= ProcessPlayerFell;

        _cameraController.OnShiftCompleted -= ProcessCameraShifting;
        _damageController.OnDamage -= ProcessLimbDamage;

        SendOnChangeEvent<PlayerStandUpState>();
    }

    public override void Tick() { }

    public override void Exit()
    {
        _ragdollFallController.Disable();
        _ragdollController.Disable();
        _damageController.Disable();
    }
}
