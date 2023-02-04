
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(Movement))]
[RequireComponent (typeof(Character))]
public class CharacterAnimations : MonoBehaviour
{
    private Animator _animator;
    private Movement _movement;
    private Character _character;

    private readonly int _verticalParameterHash = Animator.StringToHash("Vertical");
    private readonly int _horizontalParameterHash = Animator.StringToHash("Horizontal");
    private readonly int _targetModeParameterHasg = Animator.StringToHash("IsTargetMode");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<Movement>();
        _character = GetComponent<Character>();
    }

    private void Update()
    {
        _animator.SetBool(_targetModeParameterHasg, _character.IsTargetMode);
        _animator.SetFloat(_verticalParameterHash, _movement.RelativeMoveVelocity.z);
        _animator.SetFloat(_horizontalParameterHash, _movement.RelativeMoveVelocity.x);
    }
}
