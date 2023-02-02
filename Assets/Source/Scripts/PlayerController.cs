using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _cameraTarget;

    public Vector3 ViewInput { get; private set; }
    public Vector3 CameraTargetPosition => _cameraTarget.position;

    private PlayerInputActions _inputActions;
    private Player _player;
    private Vector3 _moveInput;
    private Vector3 _moveInputCameraRelative;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _player = GetComponent<Player>();

        _inputActions.Movement.Movement.performed += context => _moveInput = context.ReadValue<Vector3>();
        _inputActions.Actions.WalkingToggle.performed += context => _player.ToggleWalking();
        _inputActions.Actions.Sprint.performed += context => _player.ToggleSprint();
        _inputActions.Actions.TargetMode.performed += context => _player.ToggleTargetMode();
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

        _moveInputCameraRelative = _cameraController.transform.forward * _moveInput.z;
        _moveInputCameraRelative += _cameraController.transform.right * _moveInput.x;

        _player.ApplyMove(_moveInputCameraRelative); 
        _player.ApplyView(_cameraController.TargetRotation);
    }
}
