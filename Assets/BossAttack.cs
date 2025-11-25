using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossAttack : MonoBehaviour
{
    [Header("ターゲット")]
    public Transform player;

    [Header("プレハブ")]
    public GameObject togePrefab;   // 近接（棘）
    public GameObject bulletPrefab; // 弾（後で）
    public GameObject beamPrefab;   // ビーム（後で）

    [Header("棘の設定")]
    public float togeLifetime = 1.5f;   // 何秒で消えるか
    public Vector3 togeOffset = new Vector3(0, 0, 1.5f); // ボスの前に出す
    [Header("棘の設定")]
    public float radialSpawnRadius = 2.0f;  //← ボスからの距離を調整できる
    [Header("レーザー設定")]
    
    public float chargeTime = 2f;       // 追従時間
    public float fireDelay = 0.5f;      // 発射までの待機
    public float beamDuration = 0.15f;  // ビーム表示時間
    public int maxReflections = 3;      // 最大反射回数
    public float beamSpeed = 30f;       // 参考用、移動があれば

    private GameObject[] walls;
    void Start()
    {
        // 壁はタグ "Wall" から取得
        walls = GameObject.FindGameObjectsWithTag("Wall");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TogeRadial();   // ← 1は確定で円形8本
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(TogeLineShot());   // ← 4で確定3本
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DoBeam();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(ShootBurst());
        }

    }

    // ------------------------
    // ① 棘（近接攻撃）
    // ------------------------
    void DoToge()
    {
        int pattern = Random.Range(0, 2); // 0 or 1

        if (pattern == 0)
        {
            TogeRadial(); // 円形8本
        }
        else
        {
            TogeLineShot(); // プレイヤー直線3本
        }
    }
    IEnumerator AnimateTogeRise(GameObject toge, float riseHeight = 2f, float duration = 0.15f)
    {
        Vector3 startPos = toge.transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            toge.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    void TogeRadial()
    {
        int count = 8;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;
            Quaternion rot = Quaternion.Euler(0, angle, 0);

            Vector3 offset = rot * Vector3.forward * radialSpawnRadius;
            Vector3 spawnPos = transform.position + offset;

            // スタート位置は地面より下（-1f とかは要調整）
            Vector3 undergroundPos = spawnPos + Vector3.down * 1.5f;

            GameObject tg = Instantiate(togePrefab, undergroundPos, rot);

            StartCoroutine(AnimateTogeRise(tg));

            Destroy(tg, togeLifetime);
        }
    }

    IEnumerator TogeLineShot()
    {
        Vector3 start = transform.position;
        Vector3 end = player.position;
        Vector3 dir = (end - start).normalized;

        float[] ratios = { 0.4f, 0.6f, 0.8f };
        float[] scales = { 0.6f, 0.9f, 1.3f };

        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = start + dir * (Vector3.Distance(start, end) * ratios[i]);

            // 地面の下から
            Vector3 underground = pos + Vector3.down * 1.5f;

            GameObject tg = Instantiate(togePrefab, underground, Quaternion.LookRotation(dir));
            tg.transform.localScale *= scales[i];

            StartCoroutine(AnimateTogeRise(tg));

            // 次の刺を少し遅らせる（連続で出る）
            yield return new WaitForSeconds(0.08f);

            Destroy(tg, togeLifetime);
        }
    }




    // ------------------------
    // ② ビーム
    // ------------------------
    public void DoBeam()
    {
        StartCoroutine(BeamAttack());
    }
    IEnumerator BeamAttack()
    {
        // ① ビームオブジェクト生成
        GameObject beam = Instantiate(beamPrefab);
        LineRenderer lr = beam.GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogWarning("Beam prefab に LineRenderer が付いていません！");
            yield break;
        }

        lr.positionCount = 2; // 初期は単純な線
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, player.position);

        // ② チャージ中：2秒間プレイヤー追従
        float elapsed = 0f;
        while (elapsed < chargeTime)
        {
            Vector3 futureTarget = player.position;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, futureTarget);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ③ 発射まで待機
        yield return new WaitForSeconds(fireDelay);

        // ④ 反射ルート探索
        Vector3 target = player.position;
        List<Vector3> path = FindBeamPath(transform.position, target);

        // ⑤ ビーム描画更新
        lr.positionCount = path.Count;
        lr.SetPositions(path.ToArray());

        // ⑥ ビーム表示時間後に消す
        Destroy(beam, beamDuration);
    }

    // --------------------------
    // 壁反射を考慮したルート探索
    // --------------------------
    List<Vector3> FindBeamPath(Vector3 start, Vector3 target)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(start);

        Vector3 dir = (target - start).normalized;
        Vector3 currentPos = start;

        // プレイヤーを無視する LayerMask を作る
        int layerMask = ~(1 << LayerMask.NameToLayer("Player")); // Player レイヤーだけ無視

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentPos, dir, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    // 壁に当たった → 反射
                    dir = Vector3.Reflect(dir, hit.normal).normalized;
                    currentPos = hit.point + dir * 0.05f; // 少しずらす
                    path.Add(hit.point);
                }
                else
                {
                    // 予期しないものに当たった場合は終了
                    break;
                }
            }
            else
            {
                // Ray が何もヒットしなければ直接プレイヤーに向かう
                break;
            }
        }

        // 最後にプレイヤーに到達するポイントを追加
        path.Add(target);
        return path;
    }

    // ------------------------
    // ③ 射撃
    // ------------------------

    IEnumerator ShootBurst()
    {
        int count = 8;
        for (int i = 0; i < count; i++)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            // 弾を向ける（移動はBullet.cs側でやる）
            Instantiate(
                bulletPrefab,
                transform.position,
                Quaternion.LookRotation(dir)
            );

            yield return new WaitForSeconds(0.1f);
        }
    }


    void ShootOne()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + dir * 1.2f, Quaternion.LookRotation(dir));
    }

}
