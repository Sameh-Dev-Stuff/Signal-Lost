using UnityEngine;
using NaughtyAttributes;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private Animator animator;
    [SerializeField, Required] private InputManager input;

    [Header("Animation Parameters")]
    [SerializeField, AnimatorParam("animator")]
    private int xInput;

    [SerializeField, AnimatorParam("animator")]
    private int yInput;
    
    [SerializeField, AnimatorParam("animator")]
    private int isRuning;
    
    [SerializeField, AnimatorParam("animator")]
    private int isShooting;

    private void Update()
    {
        Animation();
    }

    private void Animation()
    {
        Vector2 moveInput = input.MoveInput();

        Vector3 worldMove = new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 localMove = transform.InverseTransformDirection(worldMove);
        
        animator.SetFloat(xInput, localMove.x);
        animator.SetFloat(yInput, localMove.z);
        animator.SetBool(isRuning, input.SprintInput());
        animator.SetBool(isShooting, input.AttackInputIsPressed());
    }
}