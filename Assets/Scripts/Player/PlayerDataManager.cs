using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Chỉ số Cơ bản")]
    public int currentHealth;
    public int maxHealth = 5; // Chỉnh lại theo số lượng tim/kiếm UI mặc định của bạn
    public int attackDamage = 100; // Sát thương mặc định

    [Header("Tài sản (Kinh tế)")]
    public int gold = 0;
    public int currentLevel = 1;
    public int currentExp = 0;

    [Header("Dữ liệu Túi đồ (Inventory)")]
    public List<InventoryItem> savedInventory = new List<InventoryItem>();

    // CÁC BIẾN ẨN ĐỂ NHỚ CHỈ SỐ GỐC LÚC MỚI VÀO GAME
    private int defaultMaxHealth;
    private int defaultAttackDamage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ghi nhớ chỉ số gốc
            defaultMaxHealth = maxHealth;
            defaultAttackDamage = attackDamage;

            currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =======================================================
    // HÀM MỚI: TẨY TRẮNG MỌI DỮ LIỆU VỀ LẠI LÚC BẮT ĐẦU CHƠI
    // =======================================================
    public void ResetData()
    {
        // 1. Phục hồi chỉ số cơ bản
        maxHealth = defaultMaxHealth;
        currentHealth = defaultMaxHealth;
        attackDamage = defaultAttackDamage;

        // 2. Tịch thu toàn bộ tài sản
        gold = 0;
        currentLevel = 1;
        currentExp = 0;

        // 3. Đốt sạch túi đồ
        if (savedInventory != null)
        {
            savedInventory.Clear();
        }

        Debug.Log("Đã xóa sạch dữ liệu người chơi!");
    }
}