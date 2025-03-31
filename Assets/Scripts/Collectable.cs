using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemName; // Name or ID of the item
    public Sprite itemIcon; // Icon for UI representation

    private void OnTriggerEnter(Collider other) 
    {   
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(this);
            Destroy(gameObject);
        }
    }

    
}