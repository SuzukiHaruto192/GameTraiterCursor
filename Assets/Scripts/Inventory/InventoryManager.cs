using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    [SerializeField] int maxSlots = 10;

    public void AddItem(ItemData newItem)
    {
        if (items.Count < maxSlots)
        {
            items.Add(newItem);
        }
    }
}
