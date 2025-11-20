using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // プレイヤー
    public float followSpeed = 10f; // 滑らかに追従する速度
    public float rotationSpeed = 100f; // 左右矢印での微調整速度

    public Vector3 offset = new Vector3(0, 5, -8); // プレイヤー相対の基本位置

    private float horizontalAngle = 0f; // 矢印微調整用

    void LateUpdate()
    {
        // 左右矢印で微調整
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontalAngle -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            horizontalAngle += rotationSpeed * Time.deltaTime;

        // プレイヤー正面方向を基準にオフセット回転
        Vector3 rotatedOffset = Quaternion.Euler(0, target.eulerAngles.y + horizontalAngle, 0) * offset;
        Vector3 desiredPos = target.position + rotatedOffset;

        // 滑らかに移動
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // 常にプレイヤーを見る
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
