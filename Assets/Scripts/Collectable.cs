using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public AudioClip collectSound;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || !other.CompareTag("Player")) return;
        isCollected = true;

        // turn off collider so no duplicates
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);

        InventoryManager.Instance.AddItem(this);
        Destroy(gameObject);
    }
}
