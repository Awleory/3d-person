
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(Movement))]
public class CharacterAnimations : MonoBehaviour
{
    private Animator _animator;
    private Movement _movement;

    private readonly int _verticalParameterHash = Animator.StringToHash("Vertical");
    private readonly int _horizontalParameterHash = Animator.StringToHash("Horizontal");
    private readonly int _targetModeParameterHasg = Animator.StringToHash("IsTargetMode");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<Movement>();   
    }

    private void Update()
    {
        _animator.SetBool(_targetModeParameterHasg, _movement.IsTargetMode);
        _animator.SetFloat(_verticalParameterHash, _movement.RelativeMoveVelocity.z);
        _animator.SetFloat(_horizontalParameterHash, _movement.RelativeMoveVelocity.x);
    }
}
