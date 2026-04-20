using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private PlayerInput input;
    private Animator animator;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 moveInput;
    private Vector3 velocity;
    
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float jumpHeight = 1.0f; 
    public float gravity = -25f;    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        if (input != null)
        {
            moveAction = input.actions.FindAction("Move");
            jumpAction = input.actions.FindAction("Jump");

            if (jumpAction != null)
                jumpAction.started += OnJump;
        }
    }

    void Update()
    {
        if (moveAction == null || controller == null) return;

        moveInput = moveAction.ReadValue<Vector2>();
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;
        
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (move.magnitude >= 0.1f)
        {
            float targetSpeed = isShiftPressed ? runSpeed : walkSpeed;
            controller.Move(move * targetSpeed * Time.deltaTime);
            
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * 10f);
        }     


        if (animator != null)
        {
            float moveMagnitude = move.magnitude;
            
            float animSpeedValue = 0;
            if (moveMagnitude > 0.1f)
            {
                animSpeedValue = isShiftPressed ? 1.0f : 0.5f;
            }
            
            animator.SetFloat("Speed", animSpeedValue);
            animator.SetBool("isRunning", isShiftPressed && moveMagnitude > 0.1f);
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (animator != null) animator.SetBool("isJumping", false);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            if (animator != null)
            {
                animator.SetTrigger("Jump");
                animator.SetBool("isJumping", true);
            }
        }
    }

    private void OnDestroy()
    {
        if (jumpAction != null)
            jumpAction.started -= OnJump;
    }
}