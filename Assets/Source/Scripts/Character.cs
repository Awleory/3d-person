
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent (typeof(CharacterAnimations))]
public class Character : MonoBehaviour
{
    private Movement _movement;
    private CharacterAnimations _characterAnimations;

    private void Awake()
    {
        _movement = GetComponent<Movement>();
        _characterAnimations = GetComponent<CharacterAnimations>();
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
        _movement.ToggleTargetMode();
    }

    public void ToggleWalking()
    {
        _movement.ToggleWalking();
    }
}
