using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item‚É“–‚½‚è‚Ü‚µ‚½");
            Activate(other.gameObject);

            // E‚í‚ê‚½ƒAƒCƒeƒ€‚ğíœ
            Destroy(gameObject);
        }
    }

    protected abstract void Activate(GameObject player);
}
