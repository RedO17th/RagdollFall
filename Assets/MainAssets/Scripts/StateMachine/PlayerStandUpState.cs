using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerStandUpState : BaseState
{
    private RagdollController _ragdollController = null;
    private AnimationController _animatorController = null;

    private float _resetBoneTime = 1f;

    private GameObject _viewGO = null;

    private BoneTransform[] _standUpFaceUpAnimationBones = null;
    private BoneTransform[] _standUpFaceDownAnimationBones = null;

    private BoneTransform[] _targetBoneTransforms = null;

    private float percent = 0f;
    private float elapsedTime = 0f;

    public PlayerStandUpState(BasePlayer player) : base(player)
    {
        _viewGO = _player.View;

        _ragdollController = _player.GetController<RagdollController>();
        _animatorController = _player.GetController<AnimationController>();
    }

    public override void Enter()
    {
        Debug.Log($"PlayerStandUpState.Enter");

        _targetBoneTransforms = GetBoneTransforms();

        _player.StartCoroutine(ResetBonesRoutine());
    }

    public override void Tick()
    { 
        
    }

    public IEnumerator ResetBonesRoutine()
    {
        while (elapsedTime < _resetBoneTime)
        {
            for (int i = 0; i < _ragdollController.BonesAmount; i++)
            {
                percent = elapsedTime / _resetBoneTime;

                _ragdollController._bones[i].localPosition = Vector3.Lerp(_ragdollController._ragdollBones[i].Position, _targetBoneTransforms[i].Position, percent);
                _ragdollController._bones[i].localRotation = Quaternion.Lerp(_ragdollController._ragdollBones[i].Rotation, _targetBoneTransforms[i].Rotation, percent);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _animatorController.Enable();
        _player.StandUp(_ragdollController.IsFaceUp);
    }

    private BoneTransform[] GetBoneTransforms()
    {
        return _ragdollController.IsFaceUp ? _ragdollController._standUpFaceUpAnimationBones : _ragdollController._standUpFaceDownAnimationBones;
    }

    public override void Exit()
    {
        Debug.Log($"PlayerStandUpState.Exit");
    }
}
