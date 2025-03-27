using UnityEngine;
using System.Collections.Generic;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;
    public List<Sprite> collectedItems = new List<Sprite>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectItem(Sprite itemSprite)
    {
        if (!collectedItems.Contains(itemSprite)) // Prevent duplicates
        {
            collectedItems.Add(itemSprite);
        }
    }
}
