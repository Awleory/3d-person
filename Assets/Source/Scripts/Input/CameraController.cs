
using static Models;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _yGimbal;

    [SerializeField] private CameraSettings _settings;

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
        int viewInputSignX = _settings.InvertedX ? -1 : 1;
        _targetRotation.y += viewInputSignX * (_playerController.ViewInput.x * _settings.SensitivityX) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_targetRotation);

        int viewInputSignY = _settings.InvertedY ? 1 : -1;
        _yGimbalRotation.x += viewInputSignY * (_playerController.ViewInput.y * _settings.SensitivityY) * Time.deltaTime;
        _yGimbalRotation.x = Mathf.Clamp(_yGimbalRotation.x, _settings.YClampMin, _settings.YClampMax);
        _yGimbal.transform.localRotation = Quaternion.Euler(_yGimbalRotation);
    }

    private void CameraFollow()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _playerController.CameraTargetPosition, ref _movementVelocity, _settings.MovementSmoothTime);
    }
}
