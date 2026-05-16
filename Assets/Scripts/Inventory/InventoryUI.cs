using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;               
using UnityEngine.UI;            

public class InventoryUI : MonoBehaviour
{
    [Header("Phần quản lý UI")]
    [SerializeField] private GameObject inventoryBackground;
    [SerializeField] private Transform container;    
    [SerializeField] private GameObject slotPrefab; 

    [Header("Phần quản lý dữ liệu")]
    [SerializeField] private InventoryManager inventory; // Kéo GameManager vào đây

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        bool isActive = inventoryBackground.activeSelf;

        if (!isActive)
        {
            inventoryBackground.SetActive(true);
            RefreshUI();

            inventoryBackground.transform.localScale = Vector3.zero;
            inventoryBackground.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            inventoryBackground.transform.DOScale(0f, 0.2f).OnComplete(() => {
                inventoryBackground.SetActive(false);
            });
        }
    }

    public void RefreshUI()
    {
        if (inventory == null || container == null || slotPrefab == null) return;

        // 1. Xóa các ô cũ
        foreach (Transform child in container)
        {
            child.DOKill();
            Destroy(child.gameObject);
        }

        // 2. Tạo ô mới cho mỗi vật phẩm
        float delay = 0f;
        foreach (InventoryItem item in inventory.currentItems)
        {
            if (item == null || item.itemData == null) continue;

            GameObject newSlot = Instantiate(slotPrefab, container);
            newSlot.GetComponent<InventorySlotUI>().GetCurrentItem(item);

            // Gán Icon
            Image itemIcon = newSlot.transform.Find("Item").GetComponent<Image>();
            if (itemIcon != null)
            {
                itemIcon.sprite = item.itemData.icon;
                itemIcon.enabled = true;
                itemIcon.color = Color.white;
            }
            else
                itemIcon.enabled = false;

            // Gán số lượng
            TextMeshProUGUI quantityText = newSlot.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
            if (quantityText != null)
            { 
                if (item.quantity > 1)
                {
                    quantityText.text = "X" + item.quantity.ToString();
                    quantityText.gameObject.SetActive(true);
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                }
            }


        // 3. Hiệu ứng nảy từng ô một (Stagger effect)
            newSlot.transform.localScale = Vector3.zero;
            newSlot.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.05f;
        }
    }
}