using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected. Adding item: " + itemName);
            InventoryManager.Instance.AddItem(this);
            Destroy(gameObject);
        }
    }   
}
