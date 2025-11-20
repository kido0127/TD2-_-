using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MoveAxis { X, Y, Z }
    public enum TriggerMode { Always, OnPlayerRide }

    [Header("移動設定")]
    public MoveAxis axis = MoveAxis.X;
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    [Header("動作タイミング")]
    public TriggerMode triggerMode = TriggerMode.Always;

    private Vector3 startPos;
    private Vector3 endPos;

    private bool isPlayerOn = false;
    private float t = 0f;

    void Start()
    {
        startPos = transform.position;

       
        Vector3 offset = Vector3.zero;
        switch (axis)
        {
            case MoveAxis.X: offset = new Vector3(moveDistance, 0, 0); break;
            case MoveAxis.Y: offset = new Vector3(0, moveDistance, 0); break;
            case MoveAxis.Z: offset = new Vector3(0, 0, moveDistance); break;
        }

        endPos = startPos + offset;
    }

    void Update()
    {
        
        if (triggerMode == TriggerMode.OnPlayerRide && !isPlayerOn) return;

       
        t += Time.deltaTime * moveSpeed;
        float lerp = (Mathf.Sin(t) + 1f) / 2f;
        transform.position = Vector3.Lerp(startPos, endPos, lerp);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = true;

           
            collision.transform.SetParent(transform);
        }
    }

    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;

            
            collision.transform.SetParent(null);
        }
    }
}
