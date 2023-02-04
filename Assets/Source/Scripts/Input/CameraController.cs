using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Character _character;
    [SerializeField] private Transform _yGimbal;

    [Header("Camera settings")]
    [SerializeField] private float _sensitivityX;
    [SerializeField] private float _sensitivityY;
    [SerializeField] private bool _invertedX;
    [SerializeField] private bool _invertedY;
    [SerializeField] private float _yClampMin = -60f;
    [SerializeField] private float _yClampMax = 60f;
    [SerializeField] private float _movementSmoothTime = 0.1f;
    [SerializeField] private float _rotateSmoothTime = 0.1f;
    [SerializeField] private bool _isTargetMode;

    public Vector3 TargetRotation => _targetRotation;

    private Vector3 _targetRotation;
    private Vector3 _yGimbalRotation;
    private Vector3 _movementVelocity;

    private void Update()
    {
        CameraRotation();
        CameraFollow();
    }

    private void CameraRotation()
    {
        if (_character.IsTargetMode && _character.Target)
        {
            _yGimbalRotation.x = 0;
            Rotate(_character.transform.rotation, Quaternion.Euler(_yGimbalRotation));
            _targetRotation.y = _character.transform.rotation.eulerAngles.y;
        }
        else
        {
            int viewInputSignX = _invertedX ? -1 : 1;
            _targetRotation.y += viewInputSignX * (_playerController.ViewInput.x * _sensitivityX) * Time.deltaTime;

            int viewInputSignY = _invertedY ? 1 : -1;
            _yGimbalRotation.x += viewInputSignY * (_playerController.ViewInput.y * _sensitivityY) * Time.deltaTime;
            _yGimbalRotation.x = Mathf.Clamp(_yGimbalRotation.x, _yClampMin, _yClampMax);

            Rotate(Quaternion.Euler(_targetRotation), Quaternion.Euler(_yGimbalRotation));
        }
    }

    private void CameraFollow()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _playerController.CameraTargetPosition, ref _movementVelocity, _movementSmoothTime);
    }

    private void Rotate(Quaternion xGimbal, Quaternion yGimbal)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, xGimbal, _rotateSmoothTime);
        _yGimbal.transform.localRotation = Quaternion.Lerp(_yGimbal.transform.localRotation, yGimbal, _rotateSmoothTime);
    }
}
