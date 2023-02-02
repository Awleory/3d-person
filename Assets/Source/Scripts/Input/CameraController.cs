using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _yGimbal;
    
    [Header("Camera settings")]
    [SerializeField] private float _sensitivityX;
    [SerializeField] private float _sensitivityY;
    [SerializeField] private bool _invertedX;
    [SerializeField] private bool _invertedY;
    [SerializeField] private float _yClampMin = -60f;
    [SerializeField] private float _yClampMax = 60f;
    [SerializeField] private float _movementSmoothTime = 0.1f;

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
        int viewInputSignX = _invertedX ? -1 : 1;
        _targetRotation.y += viewInputSignX * (_playerController.ViewInput.x * _sensitivityX) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_targetRotation);

        int viewInputSignY = _invertedY ? 1 : -1;
        _yGimbalRotation.x += viewInputSignY * (_playerController.ViewInput.y * _sensitivityY) * Time.deltaTime;
        _yGimbalRotation.x = Mathf.Clamp(_yGimbalRotation.x, _yClampMin, _yClampMax);
        _yGimbal.transform.localRotation = Quaternion.Euler(_yGimbalRotation);
    }

    private void CameraFollow()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _playerController.CameraTargetPosition, ref _movementVelocity, _movementSmoothTime);
    }
}
