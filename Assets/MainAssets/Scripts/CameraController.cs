using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : BasePlayerController
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _hipsLookTarget;
    [SerializeField] private Vector3 _cameraOffset;

    [Range(1f, 10f)]
    [SerializeField] private float _movementSpeed = 5f;

    private BasePlayer _player = null;

    private Vector3 _lookTargetPosition = Vector3.zero;
    private Vector3 _previousLookTargetPosition = Vector3.zero;

    private bool _lookTargetIsExist = false;

    private Coroutine _shiftLookRoutine = null;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _lookTargetPosition = _hipsLookTarget.position;
    }

    public void ShiftLookAt(Vector3 position)
    {
        _lookTargetIsExist = true;

        _previousLookTargetPosition = _lookTargetPosition;

        _lookTargetPosition = position;

        _shiftLookRoutine = StartCoroutine(ShiftLookRoutine());
    }

    private IEnumerator ShiftLookRoutine()
    {
        var time = 3f;
        var percent = 0f;
        var elapsedTime = 0f;

        var currentPosition = _previousLookTargetPosition;
        var targetPosition = _lookTargetPosition;

        var currentFOV = 60f;
        var targetFOV = 5f;

        while (elapsedTime < time) 
        {
            percent = elapsedTime / time;

            var newPosition = Vector3.Lerp(currentPosition, targetPosition, percent);

            _cameraTransform.transform.LookAt(newPosition);

            _camera.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, percent);

            //А время по всей видимости должно быть реальное...
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _camera.fieldOfView = targetFOV;
        _cameraTransform.transform.LookAt(targetPosition);

    }

    private void LateUpdate()
    {
        SetPositionToCamera();


        if (_lookTargetIsExist == false)
        {
            NormalLookAtTarget();
        }
    }

    //[??] Normal or Lerp?
    private void SetPositionToCamera()
    {
        //_camera.position = _hipsLookTarget.position + _cameraOffset;

        var targetPosition = _hipsLookTarget.position + _cameraOffset;

        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, _movementSpeed * Time.deltaTime);
    }

    private void NormalLookAtTarget()
    {
        _camera.fieldOfView = 60f;
        _cameraTransform.transform.LookAt(_lookTargetPosition);
    }

    public override void Clear()
    {
        _player = null;
    }
}
