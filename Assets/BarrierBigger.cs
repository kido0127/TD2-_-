using UnityEngine;

public class BarrierBigger : ItemBase
{
    public float scaleMultiplier = 2f;

    protected override void Activate(GameObject player)
    {
        // プレイヤーの子から Barrier を探す
        Transform barrier = player.transform.Find("Barrier");

        if (barrier == null)
        {
            Debug.LogWarning("Barrier がプレイヤーの子に見つからないよ");
            return;
        }

        // その場でただ大きくする
        barrier.localScale *= scaleMultiplier;
    }
}
