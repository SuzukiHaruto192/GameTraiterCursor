using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Túi đồ")]
    public int crystalCount = 0; // Số pha lê hiện có

    // Hàm này sẽ được gọi khi viên pha lê bay chạm vào người chơi
    public void AddCrystal(int amount)
    {
        crystalCount += amount;
        Debug.Log("💎 Nhặt được pha lê! Tổng số: " + crystalCount);

        // Sau này bạn có thể thêm code update UI (chữ hiển thị trên màn hình) ở đây
    }
}