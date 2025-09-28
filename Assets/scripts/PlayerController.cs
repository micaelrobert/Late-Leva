using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private Transform cameraTransform;

    // var do movimento
    [SerializeField]
    private float walkSpeed = 5.0f;
    [SerializeField]
    private float sprintSpeed = 8.0f;
    [SerializeField]
    private float rotationSpeed = 10.0f;

    // var do pulo
    [SerializeField]
    private float jumpHeight = 1.5f;
    [SerializeField]
    private float gravityValue = -9.81f;

    // var do Buffer de Pulo
    private float jumpBufferTime = 0.2f; 
    private float jumpBufferCounter;
  

    private Vector3 playerVelocity;
    private bool groundedPlayer;

    // var de dash
    [SerializeField]
    private float dashSpeed = 15.0f;
    [SerializeField]
    private float dashDuration = 0.2f;
    private float dashCooldown = 1.0f;
    private float lastDashTime = -1.0f;
    private bool isDashing = false;
    private float dashTimer;

    // Input System
    private InputSystem_Actions playerInput;
    private Vector2 movementInput = Vector2.zero;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = new InputSystem_Actions();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Dash.performed += OnDash;
        playerInput.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player.Dash.performed -= OnDash;
        playerInput.Player.Jump.performed -= OnJump;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (isDashing)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) isDashing = false;
        }
        else
        {
            movementInput = playerInput.Player.Move.ReadValue<Vector2>();
            Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);

            bool isSprinting = playerInput.Player.Sprint.IsPressed();
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            if (moveDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
                Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            float animationSpeedValue = 0f;
            if (moveDirection.magnitude > 0.1f) animationSpeedValue = isSprinting ? 1f : 0.5f;
            animator.SetFloat("moveSpeed", animationSpeedValue);
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            animator.SetTrigger("Jump");
            jumpBufferCounter = 0f; 
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            dashTimer = dashDuration;
            lastDashTime = Time.time;
        }
    }
}