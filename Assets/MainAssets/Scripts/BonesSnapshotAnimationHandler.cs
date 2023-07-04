using System.Collections.Generic;
using UnityEngine;

public class BoneSnapshot
{
    public AnimationType Type { get; private set; }
    public IReadOnlyList<BoneTransform> Bones => _snapshot;

    private BoneTransform[] _snapshot = null;

    public BoneSnapshot(AnimationType type, BoneTransform[] snapshot)
    {
        Type = type;

        _snapshot = snapshot;
    }
}

public class BonesSnapshotAnimationHandler
{
    private IReadOnlyList<Transform> _playerBones;

    private List<BoneSnapshot> _snapshots = new List<BoneSnapshot>();

    public BonesSnapshotAnimationHandler(IReadOnlyList<Transform> bones)
    {
        _playerBones = bones;
    }

    public void CreateSnapshot(AnimationType type)
    {
        _snapshots.Add(new BoneSnapshot(type, SaveBoneTransformInto(CreateSourceContainer())));
    }

    private BoneTransform[] CreateSourceContainer()
    {
        BoneTransform[] source = new BoneTransform[_playerBones.Count];

        for (int i = 0; i < source.Length; i++)
        {
            source[i] = new BoneTransform();
        }

        return source;
    }

    private BoneTransform[] SaveBoneTransformInto(BoneTransform[] destination)
    {
        for (int i = 0; i < _playerBones.Count; i++)
        {
            destination[i].SetPosition(_playerBones[i].localPosition);
            destination[i].SetRotation(_playerBones[i].localRotation);
        }

        return destination;
    }

    public IReadOnlyList<BoneTransform> GetSnapshotBy(AnimationType type)
    {
        IReadOnlyList<BoneTransform> result = null;

        foreach (var snapshot in _snapshots)
        {
            if (snapshot.Type == type)
            {
                result = snapshot.Bones;
                break;
            }
        }

        return result;
    }
}
