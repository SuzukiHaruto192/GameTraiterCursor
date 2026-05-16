using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // Danh sách lưu trữ đồ đạc trong Scene hiện tại
    public List<InventoryItem> currentItems = new List<InventoryItem>();

    // Tham chiếu đến UI để làm mới giao diện
    public InventoryUI inventoryUI;

    void Start()
    {
        // 1. TẢI DỮ LIỆU TỪ KHO VĨNH CỬU KHI VỪA VÀO SCENE
        if (PlayerDataManager.Instance != null)
        {
            currentItems = new List<InventoryItem>(PlayerDataManager.Instance.savedInventory);
        }
    }

    // ==========================================
    // HÀM NHẶT ĐỒ
    // ==========================================
    public void AddItem(ItemData newItem)
    {
        if (newItem == null) return;

        bool itemExists = false;

        // 1. Kiểm tra xem đồ đã có trong túi chưa (Nếu có thì cộng dồn)
        foreach (InventoryItem item in currentItems)
        {
            if (item.itemData == newItem)
            {
                item.quantity++;
                itemExists = true;
                break;
            }
        }

        // 2. Nếu chưa có thì tạo một ô mới hoàn toàn (Đã fix lỗi Constructor)
        if (!itemExists)
        {
            InventoryItem newInvItem = new InventoryItem(newItem, 1);
            currentItems.Add(newInvItem);
        }

        // 3. Sao lưu ngay lập tức vào Kho vĩnh cửu để đi qua Scene khác không bị mất
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.savedInventory = new List<InventoryItem>(currentItems);
        }

        // 4. Gọi UI vẽ lại các ô đồ và chạy hiệu ứng nảy
        if (inventoryUI != null)
        {
            inventoryUI.RefreshUI();
        }
    }

    // ==========================================
    // HÀM KIỂM TRA SỐ LƯỢNG (DÙNG CHO BÀN KHẮC / CRAFT ĐỒ)
    // ==========================================
    public int GetItemQuantity(ItemData itemToCheck)
    {
        foreach (InventoryItem item in currentItems)
        {
            if (item.itemData == itemToCheck)
            {
                return item.quantity;
            }
        }
        return 0; // Nếu không có viên nào thì trả về 0
    }

    // ==========================================
    // HÀM TIÊU THỤ / TRỪ ĐỒ (DÙNG KHI NÂNG CẤP / BÁN ĐỒ)
    // ==========================================
    public void ConsumeItem(ItemData itemToConsume, int amount)
    {
        for (int i = 0; i < currentItems.Count; i++)
        {
            if (currentItems[i].itemData == itemToConsume)
            {
                currentItems[i].quantity -= amount;

                // Nếu dùng hết sạch thì xóa luôn ô đồ đó khỏi túi
                if (currentItems[i].quantity <= 0)
                {
                    currentItems.RemoveAt(i);
                }
                break;
            }
        }

        // Sao lưu sự thay đổi (bị trừ đồ) vào Kho vĩnh cửu
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.savedInventory = new List<InventoryItem>(currentItems);
        }

        // Gọi UI làm mới lại giao diện (Xóa ô bị trống hoặc cập nhật lại con số)
        if (inventoryUI != null)
        {
            inventoryUI.RefreshUI();
        }
    }
}