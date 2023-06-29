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

    private Transform _lookTargetPosition = null;
    private Transform _previousLookTargetPosition = null;

    public bool _lookTargetIsExist = false;

    private Coroutine _shiftLookRoutine = null;

    public override void Initialize(BasePlayer player)
    {
        _player = player;

        _lookTargetPosition = _hipsLookTarget;
    }

    public void ShiftLookAt(Transform target)
    {
        _lookTargetIsExist = true;

        _previousLookTargetPosition = _lookTargetPosition;

        _lookTargetPosition = target;

        if (_shiftLookRoutine != null)
            StopCoroutine(_shiftLookRoutine);

        _shiftLookRoutine = StartCoroutine(ShiftLookRoutine(3f, _previousLookTargetPosition.position, _lookTargetPosition.position, 60f, 5f));
    }

    private IEnumerator ShiftLookRoutine(float timeRoutine, Vector3 currentPosition, Vector3 targetPosition, float currentFOV, float targetFOV)
    {
        var percent = 0f;
        var elapsedTime = 0f;

        while (elapsedTime < timeRoutine) 
        {
            percent = elapsedTime / timeRoutine;

            SetFOV(Mathf.Lerp(currentFOV, targetFOV, percent));
            LookAtTarget(Vector3.Lerp(currentPosition, targetPosition, percent));

            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        SetFOV(targetFOV);
        LookAtTarget(targetPosition);
    }
    private void LookAtTarget(Vector3 position) => _cameraTransform.transform.LookAt(position);
    private void SetFOV(float fov) => _camera.fieldOfView = fov;

    //public void SomeMeth()
    //{
    //    StartCoroutine(ShiftBackLookRoutine());
    //}

    private IEnumerator ShiftBackLookRoutine()
    {
        if (_shiftLookRoutine != null)
            StopCoroutine(_shiftLookRoutine);

        var currentPosition = _lookTargetPosition;
        var targetPosition = _previousLookTargetPosition;

        _shiftLookRoutine = StartCoroutine(ShiftLookRoutine(3f, currentPosition.position, targetPosition.position, 5f, 60f));

        yield return _shiftLookRoutine;

        _lookTargetPosition = targetPosition;
        _lookTargetIsExist = false;
    }

    private void LateUpdate()
    {
        SetPositionToCamera();

        if (_lookTargetIsExist == false)
        {
            LookAtTarget(_lookTargetPosition.position);
        }
    }

    //[??] Normal or Lerp?
    private void SetPositionToCamera()
    {
        //_camera.position = _hipsLookTarget.position + _cameraOffset;

        var targetPosition = _hipsLookTarget.position + _cameraOffset;

        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, _movementSpeed * Time.deltaTime);
    }



    public override void Clear()
    {
        _player = null;
    }
}
