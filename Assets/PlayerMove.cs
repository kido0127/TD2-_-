using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;

    [Header("移動パラメータ")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.6f;

    [Header("ダッシュ設定")]
    public float dashSpeed = 10f;
    public float dashAcceleration = 10f;
    public float maxStamina = 5f;
    public float staminaRecovery = 1f;

    private float currentStamina;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentStamina = maxStamina;
    }

    void FixedUpdate()
    {
        // WASD or 矢印で移動
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
        moveDir = transform.TransformDirection(moveDir);

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // ダッシュ判定（Left Shift）
        float targetSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f)
        {
            targetSpeed = dashSpeed;
            currentStamina -= Time.fixedDeltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
        }
        else
        {
            currentStamina += staminaRecovery * Time.fixedDeltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        // 速度を慣性で変化
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, dashAcceleration * Time.fixedDeltaTime);

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDir.x * currentSpeed;
        velocity.z = moveDir.z * currentSpeed;

        // ジャンプ（Space）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
        }

        rb.linearVelocity = velocity;
    }
}
