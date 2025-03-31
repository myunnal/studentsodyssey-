using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    private List<CollectibleData> collectedItems = new List<CollectibleData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep inventory between scenes
            LoadInventory(); // Load saved inventory data
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(Collectible item)
    {
        // Prevent adding duplicate items
        if (!collectedItems.Any(i => i.itemName == item.itemName))
        {
            collectedItems.Add(new CollectibleData(item.itemName, item.itemIcon));
            SaveInventory(); // Save inventory after adding
        }
    }

    public List<CollectibleData> GetCollectedItems()
    {
        return collectedItems;
    }

    // Save inventory using PlayerPrefs (or use JSON for more complex saves)
    private void SaveInventory()
    {
        List<string> itemNames = collectedItems.Select(i => i.itemName).ToList();
        string json = JsonUtility.ToJson(new CollectibleDataList(itemNames));
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    // Load inventory when restarting the game
    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryData"))
        {
            string json = PlayerPrefs.GetString("InventoryData");
            CollectibleDataList data = JsonUtility.FromJson<CollectibleDataList>(json);
            
            foreach (string itemName in data.items)
            {
                // Add stored item names back (icons would need to be reassigned)
                collectedItems.Add(new CollectibleData(itemName, null)); 
            }
        }
    }
}

// Helper class to store a list of items
[System.Serializable]
public class CollectibleDataList
{
    public List<string> items;
    public CollectibleDataList(List<string> itemList)
    {
        items = itemList;
    }
}

[System.Serializable]
public class CollectibleData
{
    public string itemName;
    public Sprite itemIcon;

    public CollectibleData(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
    }
}
