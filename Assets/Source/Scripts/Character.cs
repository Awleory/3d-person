
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent (typeof(CharacterAnimations))]
public class Character : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private Movement _movement;

    public bool IsTargetMode => _isTargetMode;
    public bool IsFocusMode => _isFocusMode;
    public Transform Target => _target;

    private bool _isTargetMode;
    private bool _isFocusMode;

    private void Awake()
    {
        _movement = GetComponent<Movement>();
    }

    public void ToggleSprint()
    {
        _movement.ToggleSprint();
    }

    public void ApplyMove(Vector3 move)
    {
        _movement.ApplyMove(move);
    }

    public void ApplyView(Vector3 view)
    {
        _movement.ApplyView(view);
    }

    public void ToggleTargetMode()
    {
        _isTargetMode = !_isTargetMode;
    }

    public void ToggleFocusMode()
    {
        _isFocusMode = !_isFocusMode;
    }

    public void ToggleWalking()
    {
        _movement.ToggleWalking();
    }
}
