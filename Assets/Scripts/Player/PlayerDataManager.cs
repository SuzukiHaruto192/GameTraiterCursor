using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Chỉ số Player")]
    public int currentHealth;
    public int maxHealth = 5;
    public int attackDamage = 100;
    public float jumpForce = 15f;

    [Header("Tài nguyên")]
    public int gold = 0;

    [Header("Bản đồ & Vị trí")]
    public List<string> unlockedPortals = new List<string>();
    public float lastX;
    public float lastY;
    public string lastSceneName;
    public bool isMan1PortalUnlocked = false;

    // ==========================================
    // ĐÃ TRẢ LẠI TÚI ĐỒ CHO INVENTORY MANAGER
    // ==========================================
    [Header("Dữ liệu Túi đồ (Inventory)")]
    [System.NonSerialized] public List<InventoryItem> savedInventory = new List<InventoryItem>();

    [Header("Cài đặt Mặc định (New Game)")]
    [SerializeField] private int defaultMaxHealth = 5;
    [SerializeField] private int defaultAttackDamage = 100;
    [SerializeField] private float defaultJumpForce = 15f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Lần đầu bật game, máu phải đầy
            if (currentHealth <= 0) currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // GameManager và SaveSystem sẽ gọi hàm này trước khi lưu file JSON
    public void UpdateCurrentPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastX = player.transform.position.x;
            lastY = player.transform.position.y;
        }
        lastSceneName = SceneManager.GetActiveScene().name;
    }

    // Gọi khi nhân vật chết
    public void ResetData()
    {
        maxHealth = defaultMaxHealth;
        currentHealth = defaultMaxHealth;
        attackDamage = defaultAttackDamage;
        jumpForce = defaultJumpForce;
        gold = 0;

        if (unlockedPortals != null) unlockedPortals.Clear();

        // Nhớ dọn sạch túi đồ khi chết
        if (savedInventory != null) savedInventory.Clear();
    }
}