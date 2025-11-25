using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 3f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearVelocity = transform.forward * speed; // 最新は velocity

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("衝突: " + collision.gameObject.name + " Layer:" + collision.gameObject.layer);

        int barrierLayer = LayerMask.NameToLayer("Barrier");
        if (collision.gameObject.layer == barrierLayer)
        {
            Debug.Log("バリアにヒット！");
            Vector3 normal = collision.contacts[0].normal;
            rb.linearVelocity = Vector3.Reflect(rb.linearVelocity, normal);
        }
    }

}
