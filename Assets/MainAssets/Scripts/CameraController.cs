using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public enum ShiftDirectionType { None = -1, Min, Max }

public class CameraController : BasePlayerController
{   
    [SerializeField] private Vector3 _cameraOffset;
    
    [Header("Camera and settings")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _minFOV = 5f;
    [SerializeField] private float _maxFOV = 60f;

    [Range(1f, 5f)]
    [SerializeField] private float _shiftLookTime = 3f;

    [Space]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _hipsLookTarget;

    [Range(1f, 10f)]
    [SerializeField] private float _movementSpeed = 5f;

    public event Action<ShiftDirectionType> OnShiftCompleted;

    private ShiftDirectionType _nextDirection = ShiftDirectionType.None;

    private Coroutine _shiftLookToRoutine = null;

    private Transform _currentTarget = null;
    private Transform _previousTarget = null;
    private Transform _newTarget = null;

    private bool _isShifting = false;

    public override void Initialize(BasePlayer player)
    {
        _currentTarget = _hipsLookTarget;
    }

    private void LateUpdate()
    {
        SetPositionToCamera();

        if (_isShifting == false)
        {
            LookAtTarget(_currentTarget.position);
        }
    }

    private void SetPositionToCamera()
    {
        var targetPosition = _hipsLookTarget.position + _cameraOffset;

        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, _movementSpeed * Time.deltaTime);
    }

    public void ShiftLookTo(Transform target)
    {
        if (_shiftLookToRoutine != null)
            StopCoroutine(_shiftLookToRoutine);

        _previousTarget = _currentTarget;
        _newTarget = target;

        _nextDirection = ShiftDirectionType.Max;

        _shiftLookToRoutine = StartCoroutine(ShiftLookToRoutine(_maxFOV, _minFOV));
    }

    private IEnumerator ShiftLookToRoutine(float currentFOV, float targetFOV)
    {
        _isShifting = true;

        var percent = 0f;
        var elapsedTime = 0f;

        while (elapsedTime < _shiftLookTime)
        {
            percent = elapsedTime / _shiftLookTime;

            SetFOV(Mathf.Lerp(currentFOV, targetFOV, percent));
            LookAtTarget(Vector3.Lerp(_currentTarget.position, _newTarget.position, percent));

            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        SetFOV(targetFOV);
        LookAtTarget(_newTarget.position);

        _currentTarget = _newTarget;
        _isShifting = false;

        OnShiftCompleted?.Invoke(_nextDirection);
    }

    private void LookAtTarget(Vector3 position) => _cameraTransform.transform.LookAt(position);
    private void SetFOV(float fov) => _camera.fieldOfView = fov;

    public void ShiftLookBack()
    {
        if (_shiftLookToRoutine != null)
            StopCoroutine(_shiftLookToRoutine);

        _newTarget = _previousTarget;

        _nextDirection = ShiftDirectionType.Min;

        _shiftLookToRoutine = StartCoroutine(ShiftLookToRoutine(_minFOV, _maxFOV));
    }

    public override void Clear()
    {
        if (_shiftLookToRoutine != null)
        {
            StopCoroutine(_shiftLookToRoutine);
        }

        _shiftLookToRoutine = null;

        _previousTarget = null;
        _currentTarget = null;
        _newTarget = null;
    }
}
