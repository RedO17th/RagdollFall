using UnityEngine;

[System.Serializable]
public class BoneTransform
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    public void SetPosition(Vector3 newPosition) => Position = newPosition;
    public void SetRotation(Quaternion newRotation) => Rotation = newRotation;
}
