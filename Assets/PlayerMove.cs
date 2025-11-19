using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;

    [Header("移動パラメータ")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.6f;

    public Vector2 moveInput;
    public Vector2 MoveInput => moveInput;

    private bool jumpInput = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Input Action に紐付け
    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move);

        Vector3 velocity = rb.linearVelocity;
        velocity.x = move.x * moveSpeed;
        velocity.z = move.z * moveSpeed;

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        if (jumpInput && isGrounded)
        {
            velocity.y = jumpForce;
            jumpInput = false;
        }

        rb.linearVelocity = velocity;
    }
}