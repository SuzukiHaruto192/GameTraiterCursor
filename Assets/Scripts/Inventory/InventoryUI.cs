using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    public void OpenInventoryUI()
    {
        inventoryPanel.SetActive(true);
        inventoryPanel.transform.localScale = Vector3.zero;

        // Hiệu ứng phóng to mượt mà khi mở túi
        inventoryPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
}
