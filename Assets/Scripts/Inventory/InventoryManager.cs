using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    [SerializeField] int maxInventorySlots = 10;
    [SerializeField] int maxSlotQuantity = 99;

    public void AddItem(ItemData newItemData)
    {
        if (newItemData == null) return;

        InventoryItem itemFound = checkItemData(newItemData);

        if (itemFound == null)
        {
            if (items.Count <= maxInventorySlots)
            {
                InventoryItem newItem = new InventoryItem(newItemData, 1);
                items.Add(newItem);
            }
        }
        else
        {
            if (itemFound.quantity < maxSlotQuantity)
            {
                itemFound.quantity++;
            }
            else
            {
                Debug.Log("Da qua so luong vat pham toi da!");
            }
        }
    }


    //Hàm ki?m tra item ???c thêm vào ?ã có data trong inventory ch?a
    public InventoryItem checkItemData(ItemData data)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemData == data)
                return item;
        }

        return null;
    }   
}
