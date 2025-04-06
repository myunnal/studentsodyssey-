using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);

            Debug.Log("Player detected. Adding item: " + itemName);
            InventoryManager.Instance.AddItem(this);
            Destroy(gameObject);
        }
    }   
}
