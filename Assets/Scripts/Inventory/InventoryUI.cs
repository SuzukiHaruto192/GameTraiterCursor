using UnityEngine;
using System.Collections.Generic; 
using DG.Tweening;               
using UnityEngine.UI;            

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryBackground;
    [SerializeField] private Transform container;    
    [SerializeField] private GameObject slotPrefab; 

    [Header("Data Reference")]
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
            Destroy(child.gameObject);
        }

        // 2. Tạo ô mới cho mỗi vật phẩm
        float delay = 0f;
        foreach (ItemData item in inventory.items)
        {
            
            GameObject newSlot = Instantiate(slotPrefab, container);

            // Gán Icon
            Image itemIcon = newSlot.transform.Find("Item").GetComponent<Image>();
            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
                itemIcon.color = Color.white;
            }
            else
                itemIcon.enabled = false;

                // 3. Hiệu ứng nảy từng ô một (Stagger effect)
            newSlot.transform.localScale = Vector3.zero;
            newSlot.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.05f; // Mỗi ô hiện sau ô trước 0.05s cho đẹp
        }
    }
}