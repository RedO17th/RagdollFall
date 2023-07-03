using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerStandUpState : BaseState
{
    private RagdollController _ragdollController = null;
    private AnimationController _animatorController = null;

    private float _resetBoneTime = 1f;

    private IReadOnlyList<Transform> _bones = null;
    private IReadOnlyList<BoneTransform> _ragdollBones = null;
    private IReadOnlyList<BoneTransform> _targetBoneTransforms = null;

    private float percent = 0f;
    private float elapsedTime = 0f;

    public PlayerStandUpState(BasePlayer player) : base(player)
    {
        _ragdollController = _player.GetController<RagdollController>();
        _animatorController = _player.GetController<AnimationController>();
    }

    public override void Enter()
    {
        Debug.Log($"PlayerStandUpState.Enter");

        _bones = _ragdollController.Bones;
        _ragdollBones = _ragdollController.RagdollBones;
        _targetBoneTransforms = _animatorController.GetStandUpAnimationBonesBySide(_ragdollController.IsFaceUp);

        _player.StartCoroutine(ResetBonesRoutine());
    }

    public override void Tick() { }

    public IEnumerator ResetBonesRoutine()
    {
        while (elapsedTime < _resetBoneTime)
        {
            for (int i = 0; i < _ragdollController.Bones.Count; i++)
            {
                percent = elapsedTime / _resetBoneTime;

                _bones[i].localPosition = Vector3.Lerp(_ragdollBones[i].Position, _targetBoneTransforms[i].Position, percent);
                _bones[i].localRotation = Quaternion.Lerp(_ragdollBones[i].Rotation, _targetBoneTransforms[i].Rotation, percent);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        //��������, ��� �����..
        _animatorController.Enable();
        _player.StandUp(_ragdollController.IsFaceUp);
    }

    public override void Exit()
    {
        Debug.Log($"PlayerStandUpState.Exit");
    }
}
