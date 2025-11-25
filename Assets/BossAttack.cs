using UnityEngine;
using System.Collections;

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
            DoShoot();
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
    // ② ビーム（ダミー）
    // ------------------------
    void DoBeam()
    {
        Debug.Log("ビーム攻撃（まだダミー）");
        // 後で溜め → レーザー を実装する
    }

    // ------------------------
    // ③ 射撃（ダミー）
    // ------------------------
    void DoShoot()
    {
        Debug.Log("弾攻撃（まだダミー）");
        // 後で弾を Instantiate → Forward に飛ばす処理を入れる
    }
}
