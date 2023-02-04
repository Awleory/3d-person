using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Transform _cameraTarget;

    public Vector3 ViewInput { get; private set; }
    public Vector3 CameraTargetPosition => _cameraTarget.position;

    private PlayerInputActions _inputActions;
    private Character _character;
    private Vector3 _moveInput;
    private Vector3 _moveInputCameraRelative;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _character = GetComponent<Character>();

        _inputActions.Movement.Movement.performed += context => _moveInput = context.ReadValue<Vector3>();
        _inputActions.Actions.WalkingToggle.performed += context => _character.ToggleWalking();
        _inputActions.Actions.Sprint.performed += context => _character.ToggleSprint();
        _inputActions.Actions.TargetMode.performed += context => _character.ToggleTargetMode();
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

        _character.ApplyMove(_moveInputCameraRelative); 
        _character.ApplyView(_cameraController.TargetRotation);
    }
}
