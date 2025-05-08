using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform inventoryUIParent; 
    public GameObject itemUIPrefab;     

    private List<Transform> itemSlots = new List<Transform>();

    void Start()
    {
        foreach (Transform slot in inventoryUIParent)
            itemSlots.Add(slot);

        UpdateInventoryUI();
    }

    void OnEnable()  => InventoryManager.OnInventoryChanged += UpdateInventoryUI;
    void OnDisable() => InventoryManager.OnInventoryChanged -= UpdateInventoryUI;

    void UpdateInventoryUI()
    {
        // 1) clear old
        foreach (var slot in itemSlots)
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);

        // 2) get data
        var items = InventoryManager.Instance.GetCollectedItems();

        // 3) populate
        for (int i = 0; i < items.Count && i < itemSlots.Count; i++)
        {
            var data = items[i];
            if (data.itemIcon == null)
            {
                Debug.LogWarning($"[InventoryUI] slot {i} skipped—no icon for '{data.itemName}'");
                continue;
            }

            var go = Instantiate(itemUIPrefab, itemSlots[i]);
            var img = go.GetComponent<Image>();
            img.sprite = data.itemIcon;
            img.preserveAspect = true;

            // stretch to fill parent slot
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
            r.localScale = Vector3.one;
        }

        Debug.Log($"[InventoryUI] Rendered {items.Count} items.");
    }
}
