using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryItem currentItem;

    public void GetCurrentItem(InventoryItem item)
    {
        currentItem = item;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && currentItem.itemData != null)
            ItemTooltipUI.Instance.ShowTooltip(currentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance.HideTooltip();
    }
}
