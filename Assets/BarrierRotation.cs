using UnityEngine;
using UnityEngine.InputSystem;

public class BarrierRotation : MonoBehaviour
{
    [Header("プレイヤー")]
    public PlayerMove playerMove;
    public Transform player;

    [Header("回転設定")]
    public float rotateSpeed = 90f;      // 左右入力で回転速度
    public float initialXAngle = 90f;    // X軸角度固定

    [Header("距離設定")]
    public float minRadius = 1f;
    public float maxRadius = 5f;
    public float adjustSpeed = 2f;       // 距離調整速度

    private float currentAngle;          // Y軸回転用
    private float currentRadius;         // プレイヤーからの距離
    private float initialY;              // プレイヤーとの相対Y

    private float adjustInput = 0f;      // -1: E, 1: Q

    void Start()
    {
        // プレイヤー中心からの初期オフセット
        Vector3 initialOffset = transform.position - player.position;

        // X/Z平面の角度
        currentAngle = Mathf.Atan2(initialOffset.x, initialOffset.z) * Mathf.Rad2Deg;

        // X/Z平面での距離
        currentRadius = new Vector2(initialOffset.x, initialOffset.z).magnitude;

        // Y方向の相対位置を保持
        initialY = initialOffset.y;

        // X軸は固定
        Vector3 euler = transform.eulerAngles;
        euler.x = initialXAngle;
        transform.eulerAngles = euler;
    }
    void Update()
    {
        // プレイヤーの左右入力で回転
        Vector2 input = playerMove.MoveInput;
        if (Mathf.Abs(input.x) > 0.01f)
        {
            currentAngle += input.x * rotateSpeed * Time.deltaTime;
        }

        // Q/E キーで距離調整
        if (Keyboard.current.qKey.isPressed) adjustInput = 1f;
        else if (Keyboard.current.eKey.isPressed) adjustInput = -1f;
        else adjustInput = 0f;

        currentRadius += adjustInput * adjustSpeed * Time.deltaTime;
        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);

        // プレイヤー中心からの位置計算
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * currentRadius;

        // Yは初期相対位置を維持
        offset.y = initialY;

        transform.position = player.position + offset;

        // プレイヤー方向を向く
        transform.LookAt(player.position);
        Vector3 angles = transform.eulerAngles;
        angles.x = initialXAngle;
        transform.eulerAngles = angles;
    }

    // Qキー押下時
    public void OnMoveBarrierCloser(InputValue value)
    {
        adjustInput = value.isPressed ? 1f : 0f;
    }

    // Eキー押下時
    public void OnMoveBarrierFarther(InputValue value)
    {
        adjustInput = value.isPressed ? -1f : 0f;
    }
}