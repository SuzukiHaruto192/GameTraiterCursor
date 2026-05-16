using UnityEngine;
using System.Collections.Generic; // BẮT BUỘC phải có dòng này để dùng List

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Chỉ số Cơ bản")]
    public int currentHealth;
    public int maxHealth = 100;
    public int attackDamage = 10;

    [Header("Tài sản (Kinh tế)")]
    public int gold = 0; // Tiền rớt từ quái
    public int currentLevel = 1;
    public int currentExp = 0;

    [Header("Dữ liệu Túi đồ (Inventory)")]
    // Sửa ItemData thành InventoryItem
    public List<InventoryItem> savedInventory = new List<InventoryItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Khởi tạo máu lúc mới New Game
            currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}