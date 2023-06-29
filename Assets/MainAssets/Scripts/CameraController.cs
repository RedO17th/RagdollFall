using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : BasePlayerController
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _hipsLookTarget;
    [SerializeField] private Vector3 _cameraOffset;

    [Range(1f, 10f)]
    [SerializeField] private float _movementSpeed = 5f;

    private BasePlayer _player = null;

    public override void Initialize(BasePlayer player)
    {
        _player = player;
    }

    private void LateUpdate()
    {
        SetPositionToCamera();

        LookAtTarget();
    }

    //[??] Normal or Lerp?
    private void SetPositionToCamera()
    {
        //_camera.position = _hipsLookTarget.position + _cameraOffset;

        _camera.position = Vector3.Lerp(_camera.position, _hipsLookTarget.position + _cameraOffset, _movementSpeed * Time.deltaTime);
    }

    private void LookAtTarget()
    {
        _camera.transform.LookAt(_hipsLookTarget.position);
    }

    public override void Clear()
    {
        _player = null;
    }
}
