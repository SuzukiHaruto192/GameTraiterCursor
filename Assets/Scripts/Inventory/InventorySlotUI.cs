using UnityEngine;
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
        ItemTooltipUI.Instance.ShowTooltip(currentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance.HideTooltip();
    }
}
