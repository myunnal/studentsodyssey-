using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public delegate void InventoryChanged();
    public static event InventoryChanged OnInventoryChanged;

    private List<CollectibleData> collectedItems = new List<CollectibleData>();
    private const string PREF_KEY = "InventoryData";

    private void Awake()
    {
        // ONLY for testing in the Editor: wipe out old saves so you always start empty
        #if UNITY_EDITOR
        PlayerPrefs.DeleteKey("InventoryData");
        #endif

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void AddItem(Collectible item)
    {
        if (collectedItems.Any(i => i.itemName == item.itemName))
        {
            Debug.Log($"[InventoryManager] '{item.itemName}' already in inventory, skipping.");
            return;
        }

        Sprite icon = item.itemIcon ?? Resources.Load<Sprite>($"ItemIcons/{item.itemName}");
        Debug.Log($"[InventoryManager] Adding '{item.itemName}' → icon loaded? {icon != null}");
        collectedItems.Add(new CollectibleData(item.itemName, icon));

        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

    public List<CollectibleData> GetCollectedItems() => collectedItems;

    private void SaveInventory()
    {
        var names = collectedItems.Select(d => d.itemName).ToList();
        string json = JsonUtility.ToJson(new CollectibleDataList(names));
        PlayerPrefs.SetString(PREF_KEY, json);
        PlayerPrefs.Save();
        Debug.Log($"[InventoryManager] Saved {names.Count} items.");
    }

    private void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(PREF_KEY))
        {
            Debug.Log("[InventoryManager] No saved inventory.");
            return;
        }

        string json = PlayerPrefs.GetString(PREF_KEY);
        var dataList = JsonUtility.FromJson<CollectibleDataList>(json);

        // Clean out any entries whose sprite no longer exists
        List<string> validNames = new List<string>();
        foreach (var name in dataList.items)
        {
            var icon = Resources.Load<Sprite>($"ItemIcons/{name}");
            if (icon != null)
            {
                collectedItems.Add(new CollectibleData(name, icon));
                validNames.Add(name);
            }
            else
            {
                Debug.LogWarning($"[InventoryManager] Dropping saved '{name}' — icon missing in Resources.");
            }
        }

        // If we trimmed anything, re-save the cleaned list
        if (validNames.Count != dataList.items.Count)
        {
            string cleanJson = JsonUtility.ToJson(new CollectibleDataList(validNames));
            PlayerPrefs.SetString(PREF_KEY, cleanJson);
            PlayerPrefs.Save();
            Debug.Log("[InventoryManager] Cleaned up invalid saved entries and re-saved inventory.");
        }

        Debug.Log($"[InventoryManager] Loaded {collectedItems.Count} valid items.");
    }
}

[System.Serializable]
public class CollectibleDataList
{
    public List<string> items;
    public CollectibleDataList(List<string> items) { this.items = items; }
}

[System.Serializable]
public class CollectibleData
{
    public string itemName;
    public Sprite itemIcon;
    public CollectibleData(string n, Sprite i) { itemName = n; itemIcon = i; }
}
