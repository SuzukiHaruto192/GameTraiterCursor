[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        quantity = amount;
    }
}
