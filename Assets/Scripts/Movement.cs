using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Vector3 moveDirection;
    private bool shouldJump = false;
    private bool isGrounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckMoveDirection();

        if (Input_Manager._INPUT_MANAGER.GetSouthButtonPressed() && isGrounded)
        {
            shouldJump = true;
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

    private bool CheckGround()
    {
        // Offset the ray origin slightly above the character's base
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        // Create the ray with the offset origin pointing downward
        Ray groundRay = new Ray(rayOrigin, Vector3.down);

        // For visual debugging in the Unity Scene view
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);

        // Check if the ray intersects with the ground within the given distance
        bool grounded = Physics.Raycast(groundRay, groundCheckDistance, groundLayer);
        return grounded;
    }

    private void CheckMoveDirection()
    {
         
    }
    private void MoveCharacter()
    {
        bool moveRight = Input_Manager._INPUT_MANAGER.GetRightPressed();
        bool moveLeft = Input_Manager._INPUT_MANAGER.GetLeftPressed();
        bool moveUp = Input_Manager._INPUT_MANAGER.GetUpPressed();
        bool moveDown = Input_Manager._INPUT_MANAGER.GetDownPressed();

        moveDirection =
            ((moveRight ? transform.right : (moveLeft ? -transform.right : new Vector3(0f, 0f, 0f))) +
            new Vector3(0f, 0, 0f) +
            (moveUp ? transform.forward : (moveDown ? -transform.forward : new Vector3(0f, 0f, 0f)))).normalized;

        rb.velocity = moveDirection * moveSpeed;
    }

    private void Jump()
    {
        Vector3 jumpVector = new Vector3(0, jumpForce, 0);
        rb.AddForce(jumpVector, ForceMode.Impulse);
    }
}
