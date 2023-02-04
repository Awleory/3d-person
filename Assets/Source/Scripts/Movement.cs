
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float _walkingSpeed = 3f;
    [SerializeField] private float _walkingBackwardSpeed = 2f;
    [SerializeField] private float _walkingStrafeSpeed = 2f;
    [SerializeField] private float _runningSpeed = 6f;
    [SerializeField] private float _runningBackwardSpeed = 4f;
    [SerializeField] private float _runningStrafeSpeed = 4f;
    [SerializeField] private float _sprintingSpeed = 9f;
    [SerializeField] private float _movementSpeedRate = 1f;
    [SerializeField] private float _movementSmoothdamp = 0.2f;
    [SerializeField] private float _rotationSmoothdamp = 0.6f;

    [Header("Stamina")]
    [SerializeField] private float _sprintStaminaDrain;
    [SerializeField] private float _minStaminaForSprint;
    private float _stamina;
    private float _maxStamina = 100f;
    private float _staminaRestore = 5;
    private float _staminaRestoreDelay = 1f;
    private float _staminaRestoreCurrentDelay;

    public Vector3 RelativeMoveVelocity => _localMoveVelocity;

    private bool _isWalking;
    private bool _isSprinting;

    private Vector3 _moveInput;
    private Vector3 _viewTargetRotation;
    private Vector3 _moveVelocity;
    private Vector3 _localMoveVelocity;

    private float _xVelocity;
    private float _zVelocity;

    private CharacterController _characterController;
    private Character _character;

    private const float _sprintFalloff = 0.2f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _character = GetComponent<Character>();
        _stamina = _maxStamina;
    }

    private void Update()
    {
        CalculateMove();
        CalculateRotation();
        CalculateSprint();

        _characterController.Move(_moveVelocity * Time.deltaTime);
    }

    public void ApplyMove(Vector3 move)
    {
        _moveInput = move;
    }

    public void ApplyView(Vector3 view)
    {
        _viewTargetRotation = view;
    }

    public void ToggleWalking()
    {
        _isWalking = !_isWalking;
    }

    public void ToggleSprint()
    {
        if (CanSprint() == false)
            return;

        if (_stamina >= _minStaminaForSprint)
        {
            _isSprinting = true;
            _isWalking = false;
        }
    }

    private void CalculateMove()
    {
        float targetSpeed = CalculateSpeed();
        targetSpeed *= _movementSpeedRate;

        _moveVelocity.x = SmoothDamp(_moveVelocity.x, _moveInput.x * targetSpeed, ref _xVelocity, _movementSmoothdamp);
        _moveVelocity.z = SmoothDamp(_moveVelocity.z, _moveInput.z * targetSpeed, ref _zVelocity, _movementSmoothdamp);
        Debugger.Log("_moveVelocity magnitude", _moveVelocity.magnitude);
        _localMoveVelocity = Vector3.zero;
        if (_character.IsTargetMode)
            _localMoveVelocity = transform.InverseTransformDirection(_moveVelocity);
        else
            _localMoveVelocity.z = _moveVelocity.magnitude;
    }

    private void CalculateSprint()
    {
        if (CanSprint() == false)
        {
            _isSprinting = false;
        }

        if (_isSprinting)
        {
            if (_stamina > 0)
                _stamina = Mathf.Max(_stamina - _sprintStaminaDrain * Time.deltaTime, 0);
            else
                _isSprinting = false;

            _staminaRestoreCurrentDelay = _staminaRestoreDelay;
        }
        else if (_stamina <= _maxStamina)
        {
            if (_staminaRestoreCurrentDelay <= 0)
                _stamina = Mathf.Min(_stamina + _staminaRestore * Time.deltaTime, _maxStamina);
            else
                _staminaRestoreCurrentDelay = Mathf.Max(_staminaRestoreCurrentDelay - Time.deltaTime, 0);
        }
    }

    private float CalculateSpeed()
    {
        if (_moveInput.magnitude == 0)
            return 0;

        float speed;
        if (_character.IsTargetMode)
        {
            if (_moveInput.z > 0)
                speed = _isWalking ? _walkingStrafeSpeed : _runningStrafeSpeed;
            else
                speed = _isWalking ? _walkingBackwardSpeed : _runningBackwardSpeed;
        }
        else
        {
            if (_isSprinting)
                speed = _sprintingSpeed;
            else
                speed = _isWalking ? _walkingSpeed : _runningSpeed;
        }

        return speed;
    }

    private void CalculateRotation()
    {
        if (_character.IsTargetMode)
        {
            Quaternion currentRotation = transform.rotation;

            if (_character.Target)
            {
                RotateToTarget(_character.Target.position - transform.position, _rotationSmoothdamp);
            }
            else
            {
                Vector2 newRotation = currentRotation.eulerAngles;
                newRotation.y = _viewTargetRotation.y;

                RotateToTarget(Quaternion.Euler(newRotation), _rotationSmoothdamp);
            }
        }
        else
        {
            RotateToTarget(_moveVelocity.normalized, _rotationSmoothdamp);
        }
    }

    private void RotateToTarget(Vector3 target, float smoothdamp)
    {
        if (target == Vector3.zero)
            return;

        target.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(target);
        RotateToTarget(targetRotation, smoothdamp);
    }

    private void RotateToTarget(Quaternion rotation, float smoothdamp)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothdamp);
    }

    private bool CanSprint()
    {
        if (_character.IsTargetMode)
            return false;

        if (Mathf.Abs(_moveInput.z) < _sprintFalloff && Mathf.Abs(_moveInput.x) < _sprintFalloff)
            return false;

        return true;
    }

    private float SmoothDamp(float value, float targetValue, ref float currentVelocity, float smoothTime, float digitsAccuracy = 0.01f)
    {
        if (value == targetValue)
            return value;
        else if (Mathf.Abs(targetValue - value) <= digitsAccuracy)
            return targetValue;
        else
            return Mathf.SmoothDamp(value, targetValue, ref currentVelocity, smoothTime);
    }
}
