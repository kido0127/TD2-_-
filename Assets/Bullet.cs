using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 3;
    public float radius = 0.1f; // SphereCastの半径

    private Rigidbody rb;
    private Vector3 lastPos;

    // 反射直後に再反射されないようにするフラグ
    private bool justReflected = false;
    private float reflectCooldown = 0.02f; // 20ms

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = transform.forward * speed; // linearVelocityを使用
        rb.linearVelocity = transform.forward * speed; // linearVelocityを使用
        lastPos = transform.position;
        // 弾同士の衝突を無視
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (var b in bullets)
        {
            if (b != gameObject)
            {
                Physics.IgnoreCollision(b.GetComponent<Collider>(), GetComponent<Collider>());
            }
        }

        lastPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;
        Vector3 moveDir = currentPos - lastPos;
        float moveDist = moveDir.magnitude;

        if (moveDist > 0f)
        {
            int layerMask = LayerMask.GetMask("Barrier"); // Barrierレイヤーだけ
            if (!justReflected && Physics.SphereCast(lastPos, radius, moveDir.normalized, out RaycastHit hit, moveDist, layerMask))
            {
                Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, hit.normal);

                // 衝突位置補正
                transform.position = hit.point + reflectDir * 0.05f;
                rb.linearVelocity = reflectDir * speed;

                // 反射フラグON
                justReflected = true;
                Invoke(nameof(ResetReflectionFlag), reflectCooldown);
            }
        }

        lastPos = transform.position;
    }

    void ResetReflectionFlag()
    {
        justReflected = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            // 直接衝突しても反射
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflectDir * speed;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by bullet!");
            Destroy(gameObject);
        }
        else
        {
            // 地面やその他に当たったら消滅
            Destroy(gameObject);
        }
    }
}
