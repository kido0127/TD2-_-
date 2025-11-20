using UnityEngine;

public class BossMove : MonoBehaviour
{
    [Header("ターゲット")]
    public Transform player;

    [Header("距離設定")]
    public float minDistance = 5f;   // プレイヤーに近づきすぎない距離
    public float maxDistance = 10f;  // プレイヤーから離れすぎない距離

    [Header("スピード設定")]
    public float minSpeed = 2f;
    public float maxSpeed = 6f;
    public float acceleration = 3f;  // 速度変化の速さ

    [Header("行動切替")]
    public float actionIntervalMin = 1f;
    public float actionIntervalMax = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float currentSpeed = 0f;
    private float nextActionTime = 0f;

    void Start()
    {
        ChooseNextAction();
    }

    void Update()
    {
        if (!player) return;

        Vector3 horizontalPlayerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 horizontalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 toPlayer = horizontalPlayerPos - horizontalPos;
        float distance = toPlayer.magnitude;

        // 距離に応じて方向決定
        if (distance < minDistance)
        {
            moveDirection = -toPlayer.normalized; // 離れる
        }
        else if (distance > maxDistance)
        {
            moveDirection = toPlayer.normalized;  // 近づく
        }
        else
        {
            // ランダム行動を設定
            if (Time.time >= nextActionTime)
            {
                ChooseNextAction();
            }
        }

        // スピード調整（滑らかに）
        float targetSpeed = maxSpeed;
        if (moveDirection == Vector3.zero) targetSpeed = 0f;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // 移動
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        // プレイヤー方向を向く
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, 0.1f);
        }
    }

    void ChooseNextAction()
    {
        // たまに少しランダムな方向に動く
        float angle = Random.Range(-45f, 45f);
        moveDirection = Quaternion.Euler(0, angle, 0) * moveDirection;
        nextActionTime = Time.time + Random.Range(actionIntervalMin, actionIntervalMax);
    }
}
