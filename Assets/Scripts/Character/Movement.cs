using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    private float moveSpeed;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Vector3 moveDirection;
    private bool shouldJump = false;
    private bool isGrounded = false;

    [SerializeField] private Transform cameraTransform; // referencia a la transformada de la cámara
    private float crouchHeight = 1.5f; // altura del collider cuando el jugador se agacha
    private float standHeight = 2f; // altura normal del collider
    private float crouchCameraOffset = -0.5f; // cuánto se baja la cámara al agacharse

    private CapsuleCollider capsuleCollider;
    private Vector3 cameraStandPosition; // posición original de la cámara

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraStandPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        if (Input_Manager._INPUT_MANAGER.GetJump() && isGrounded)
        {
            shouldJump = true;
        }

        if (Input_Manager._INPUT_MANAGER.GetCrouch())
        {
            Crouch();
        }
        else
        {
            Stand();
        }

        if (Input_Manager._INPUT_MANAGER.GetSprint())
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    void FixedUpdate()
    {
        isGrounded = CheckGround();

        MoveCharacter();

        if (shouldJump)
        {
            Jump();
            shouldJump = false;
        }
    }

    private void Crouch()
    {
        capsuleCollider.height = crouchHeight;
    }

    private void Stand()
    {
        capsuleCollider.height = standHeight;
    }

    // Checks if the character is on the ground
    private bool CheckGround()
    {
        // Offset the ray origin slightly above the character's base
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        // Create the ray with the offset origin pointing downward
        Ray groundRay = new Ray(rayOrigin, Vector3.down);

        // For visual debugging in the Unity Scene view
        //Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);

        // Check if the ray intersects with the ground within the given distance
        bool grounded = Physics.Raycast(groundRay, groundCheckDistance, groundLayer);
        return grounded;
    }

    // Checks how the player wants to move and performs the movement
    private void MoveCharacter()
    {
        // Check where does the player want to move using the input manager script
        bool moveRight = Input_Manager._INPUT_MANAGER.GetRightPressed();
        bool moveLeft = Input_Manager._INPUT_MANAGER.GetLeftPressed();
        bool moveUp = Input_Manager._INPUT_MANAGER.GetUpPressed();
        bool moveDown = Input_Manager._INPUT_MANAGER.GetDownPressed();

        // This vector3 is the direction where the character shoud move
        moveDirection =
            ((moveRight ? transform.right : (moveLeft ? -transform.right : new Vector3(0f, 0f, 0f))) +             //x
            (moveUp ? transform.forward : (moveDown ? -transform.forward : new Vector3(0f, 0f, 0f)))).normalized;  //z

        // Moves the player to the right direction at the right speed, if the player is in the air he moves slower
        rb.velocity = isGrounded ? moveDirection * moveSpeed + new Vector3(0f, rb.velocity.y, 0f) : rb.velocity + moveDirection * moveSpeed * 0.025f;
    }

    // Makes the character jump
    private void Jump()
    {
        Vector3 jumpVector = new Vector3(0, jumpForce, 0);
        rb.AddForce(jumpVector, ForceMode.Impulse);
    }
}