using UnityEngine;

public class Rail : MonoBehaviour
{
    public Rail nextRail;        // 次のレール（時計回り or 半時計回りで進む方向）
    public Rail oppositeRail;    // 内外切り替え先
    public bool isOuter;         // 外側レールかどうか
    public string direction;     // "Up" "Down" "Left" "Right"

    public Vector3 GetPositionOnRail(float t)
    {
        // Planeの端から端までの補間位置を返す（例：中心から端）
        Vector3 start = transform.position - transform.right * 5f; // 仮に横10m
        Vector3 end = transform.position + transform.right * 5f;
        return Vector3.Lerp(start, end, t);
    }
}
