using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryUIParent; // The grid containing item slots
    public GameObject itemUIPrefab; // The UI prefab representing each item
    public List<Transform> itemSlots = new List<Transform>(); // List of all item slots

    private void Start()
    {
        // Find all child slots in inventoryUIParent (assuming they are empty Image components)
        foreach (Transform slot in inventoryUIParent)
        {
            itemSlots.Add(slot);
        }

        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        List<CollectibleData> collectedItems = InventoryManager.Instance.GetCollectedItems();
        
        for (int i = 0; i < collectedItems.Count && i < itemSlots.Count; i++)
        {
            GameObject itemUI = Instantiate(itemUIPrefab, itemSlots[i]); // Spawn item in slot
            Image itemImage = itemUI.GetComponent<Image>(); // Get Image component
            
            if (itemImage != null) 
            {
                itemImage.sprite = collectedItems[i].itemIcon; // Assign the correct icon
            }
            else
            {
                Debug.LogWarning("itemUIPrefab is missing an Image component!");
            }
        }
    }

}
