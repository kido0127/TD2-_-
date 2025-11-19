using UnityEngine;

public class PlayerRailMove : MonoBehaviour
{
    public Rail currentRail;
    public float moveSpeed = 2f;
    private float t = 0f;

    void Update()
    {
        if (currentRail == null) return;

        t += Time.deltaTime * moveSpeed * 0.1f;

        if (t >= 1f)
        {
            t = 0f;
            currentRail = currentRail.nextRail;
        }

        transform.position = currentRail.GetPositionOnRail(t);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentRail.oppositeRail != null)
            {
                currentRail = currentRail.oppositeRail;
                t = 1f - t;
            }
        }
    }
}
