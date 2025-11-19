using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // プレイヤー
    public float followSpeed = 10f;

    private Vector3 offset;    // 初期の位置差

    void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }
}