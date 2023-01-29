
using static Models;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _cameraTarget;

    [SerializeField] private CharacterSettings _settings;
    [Header("Movement")]
    [SerializeField] private bool _isTargetMode;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isSprinting;
    [SerializeField] private float _movementSpeedRate = 1f;
    [SerializeField] private float _movementSmoothdamp = 0.05f;

    [Header("Stats")]
    [SerializeField] private CharacterStats _stats;

    [Header("Gravity")]
    [SerializeField] private float _gravity = 10;
    [SerializeField] private float _maxGravity = -15;
    [SerializeField] private float _constantGravity = -0.6f;

    [Header("Combat")]
    [SerializeField] private bool _isFaceTarget;
    [SerializeField] private Transform _target;

    public Vector2 MoveInput { get; private set; }
    public Vector2 ViewInput { get; private set; }
    public Vector3 CameraTargetPosition => _cameraTarget.position;
    public bool IsTargetMode => _isTargetMode;
    public bool IsWalking => _isWalking;
    public bool IsSprinting => _isSprinting;

    private PlayerInputActions _inputActions;
    private CharacterController _characterController;
    private Animator _animator;

    private Vector3 _moveVelocity;
    private float _verticalSpeed;
    private float _targetVerticalSpeed;
    private float _verticalSpeedVelocity;

    private float _horizontalSpeed;
    private float _targetHorizontalSpeed;
    private float _horizontalSpeedVelocity;

    private float _currentGravity;
    private Vector3 _gravityDirection;
    private Vector3 _gravityMovement;

    private Vector3 _cameraRelativeForward;
    private Vector3 _cameraRelativeRight;

    private const float _sprintFalloff = 0.2f;
    private const string _verticalParameter = "Vertical";
    private const string _horizontalParameter = "Horizontal";
    private const string _targetModeParameter = "IsTargetMode";

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _inputActions.Movement.Movement.performed += context => MoveInput = context.ReadValue<Vector2>();
        _inputActions.Actions.Jump.performed += context => Jump();
        _inputActions.Actions.WalkingToggle.performed += context => ToggleWalking();
        _inputActions.Actions.Sprint.performed += context => Sprint();
        _inputActions.Actions.TargetMode.performed += context => ToggleTargetMode();

        _gravityDirection = Vector3.down;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Update()
    {
        ViewInput = _inputActions.Movement.View.ReadValue<Vector2>();

        CalculateGravity();
        CalculateMove();
        CalculateSprint();

        _characterController.Move(_moveVelocity * Time.deltaTime + _gravityMovement);
    }

    private bool IsGrounded()
    {
        return _characterController.isGrounded;
    }

    private void CalculateGravity()
    {
        if (IsGrounded())
        {
            _currentGravity = _constantGravity;
        }
        else
        {
            if (_currentGravity > _maxGravity)
            {
                _currentGravity -= _gravity * Time.deltaTime;
            }
        }

        _gravityMovement = _gravityDirection * -_currentGravity * Time.deltaTime;
    }

    private void CalculateMove()
    {
        _animator.SetBool(_targetModeParameter, _isTargetMode);
        Debug.Log(ViewInput);
        float currentSpeed;

        if (_isTargetMode)
        {
            if (MoveInput.y > 0)
                currentSpeed = IsWalking ? _settings.WalkingSpeed : _settings.RunningSpeed;
            else
                currentSpeed = IsWalking ? _settings.WalkingBackwardSpeed : _settings.RunningBackwardSpeed;

            Quaternion currentRotation;

            if (_isFaceTarget && _target)
            {
                Vector3 lookDirection = _target.position - transform.position;
                lookDirection.y = 0;

                currentRotation = transform.rotation;

                transform.LookAt(lookDirection + transform.position, Vector3.up);
                Quaternion newRotation = transform.rotation;

                transform.rotation = Quaternion.Lerp(currentRotation, newRotation, _settings.CharacterRotationSmoothdamp);
            }
            else
            {
                currentRotation = transform.rotation;

                Vector2 newRotation = currentRotation.eulerAngles;
                newRotation.y = _cameraController.TargetRotation.y;

                currentRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(newRotation), _settings.CharacterRotationSmoothdamp);

                transform.rotation = currentRotation;
            }
        }
        else
        {
            Quaternion originalRotation = transform.rotation;
            transform.LookAt(_moveVelocity + transform.position, Vector3.up);
            Quaternion newRotation = transform.rotation;

            transform.rotation = Quaternion.Lerp(originalRotation, newRotation, _settings.CharacterRotationSmoothdamp);

            if (_isSprinting)
                currentSpeed = _settings.SprintingSpeed;
            else
                currentSpeed = IsWalking ? _settings.WalkingSpeed : _settings.RunningSpeed;
        }

        currentSpeed *= _movementSpeedRate;

        _targetVerticalSpeed = currentSpeed * MoveInput.y;
        _targetHorizontalSpeed = currentSpeed * MoveInput.x;

        _verticalSpeed = SmoothDamp(_verticalSpeed, _targetVerticalSpeed, ref _verticalSpeedVelocity, _movementSmoothdamp);
        _horizontalSpeed = SmoothDamp(_horizontalSpeed, _targetHorizontalSpeed, ref _horizontalSpeedVelocity, _movementSmoothdamp);

        _cameraRelativeForward = _cameraController.transform.forward;
        _cameraRelativeRight = _cameraController.transform.right;

        _moveVelocity = _cameraRelativeForward * _verticalSpeed;
        _moveVelocity += _cameraRelativeRight * _horizontalSpeed;

        if (_isTargetMode)
        {
            Vector3 relativeMovement = transform.InverseTransformDirection(_moveVelocity);

            _animator.SetFloat(_verticalParameter, relativeMovement.z);
            _animator.SetFloat(_horizontalParameter, relativeMovement.x);
        }
        else
        {
            _animator.SetFloat(_verticalParameter, Mathf.Max(Mathf.Abs(_verticalSpeed), Mathf.Abs(_horizontalSpeed)));
        }
    }

    private void CalculateSprint()
    {
        if (CanSprint() == false)
        {
            _isSprinting = false;
        }

        if (_isSprinting)
        {
            if (_stats.Stamina > 0)
            {
                _stats.Stamina = Mathf.Max(_stats.Stamina - _stats.StaminaDrain * Time.deltaTime, 0);
            }
            else
            {
                _isSprinting = false;
            }

            _stats.StaminaRestoreCurrentDelay = _stats.StaminaRestoreDelay;
        }
        else if (_stats.Stamina <= _stats.MaxStamina)
        {
            if (_stats.StaminaRestoreCurrentDelay <= 0)
                _stats.Stamina = Mathf.Min(_stats.Stamina + _stats.StaminaRestore * Time.deltaTime, _stats.MaxStamina);
            else
                _stats.StaminaRestoreCurrentDelay = Mathf.Max(_stats.StaminaRestoreCurrentDelay - Time.deltaTime, 0);
        }
    }

    private void Jump()
    {
        Debug.Log("I'm jumping");
    }

    private void ToggleTargetMode()
    {
        _isTargetMode = !_isTargetMode;
    }

    private void ToggleWalking()
    {
        _isWalking = !_isWalking;
    }

    private void Sprint()
    {
        if (CanSprint() == false)
            return;

        if (_stats.Stamina >= _stats.MinStaminaForSprint)
        {
            _isSprinting = true;
            _isWalking = false;
        }
    }

    private bool CanSprint()
    {
        if (_isTargetMode)
            return false;

        if (Mathf.Abs(MoveInput.y) < _sprintFalloff && Mathf.Abs(MoveInput.x) < _sprintFalloff)
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
