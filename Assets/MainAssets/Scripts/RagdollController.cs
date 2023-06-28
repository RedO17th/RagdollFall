using NTC.Global.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    [Space]
    [SerializeField] private bool _isLocked = true;

    [SerializeField] private Rigidbody _hipsRigidBody;
    [SerializeField] private RagdollOperations _ragdollOperations;

    public event Action OnFallCompleted;

    private MovementController _movementController = null;
    private AnimationController _animatorController = null;

    private BasePlayer _player = null;

    private bool _playerIsFalling = false;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _movementController = _player.GetController<MovementController>();
        _animatorController = _player.GetController<AnimationController>();

        //..
        _bones = _hipsRigidBody.transform.GetComponentsInChildren<Transform>();
        _standUpAnimationBones = new BoneTransform[_bones.Length];
        _ragdollBones = new BoneTransform[_bones.Length];

        for (int i = 0; i < _bones.Length; i++)
        {
            _standUpAnimationBones[i] = new BoneTransform();
            _ragdollBones[i] = new BoneTransform();
        }

        PopulateAnimationBonesTransform(_standUpAnimationBones);
    }

    public void Unlock() => _isLocked = false;
    public void Lock() => _isLocked = true;

    public override void Enable()
    {
        if (_isLocked == false)
        {
            _player.SetState(PlayerState.Fall);

            _movementController.Disable();
            _animatorController.Disable();

            _ragdollOperations.EnableRagdoll();

            base.Enable();
        }
    }

    private void Update() => CheckFallComplition();
    private void CheckFallComplition()
    {
        CheckFallBegin();

        CheckFallEnd();
    }

    private void CheckFallBegin()
    {
        if (_playerIsFalling == false && _hipsRigidBody.velocity.magnitude >= 0.2f)
        {
            _playerIsFalling = true;

            Debug.Log($"RagdollController.CheckFallComplition: Начал падать");
        }
    }

    private void CheckFallEnd()
    {
        if (_playerIsFalling && _hipsRigidBody.velocity.magnitude <= 0.05f)
        {
            Debug.Log($"RagdollController.CheckFallComplition: Упал");

            _playerIsFalling = false;

            PopulateBoneTransform(_ragdollBones);

            //А нужно ли... Или просто использовать SetState(PlayerState.Death);
            OnFallCompleted?.Invoke();
        }
    }

    public override void Disable()
    {
        //base.Disable();

        _ragdollOperations.DisableRagdoll();
    }

    public override void Clear()
    {
        _movementController = null;
        _animatorController = null;

        _player = null;
    }

    //Bones question
    public BoneTransform[] _standUpAnimationBones;
    public BoneTransform[] _ragdollBones;
    public Transform[] _bones;
    public GameObject _viewGO;

    private void PopulateBoneTransform(BoneTransform[] destination)
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            destination[i].Position = _bones[i].localPosition;
            destination[i].Rotation = _bones[i].localRotation;
        }
    }

    private void PopulateAnimationBonesTransform(BoneTransform[] destination)
    {
        var positionBeforeSampling = transform.position;
        var rotationBeforeSampling = transform.rotation;

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "StandUP")
            {
                Debug.Log($"RagdollController.PopulateAnimationBonesTransform: True ");

                clip.SampleAnimation(_viewGO, 0f);

                PopulateBoneTransform(destination);

                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

    public IEnumerator ResetBonesRoutine()
    {
        Debug.Log($"RagdollController.ResetBonesRoutine");

        float t = 3f;
        float etime = 0f;
        float percent = 0f;

        while (etime < t)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                percent = etime / t;

                _bones[i].localPosition = Vector3.Lerp(_ragdollBones[i].Position, _standUpAnimationBones[i].Position, percent);
                _bones[i].localRotation = Quaternion.Lerp(_ragdollBones[i].Rotation, _standUpAnimationBones[i].Rotation, percent);
            }

            etime += Time.deltaTime;

            yield return null;
        }


    }
}

[System.Serializable]
public class BoneTransform
{
    public Vector3 Position;
    public Quaternion Rotation;
}
