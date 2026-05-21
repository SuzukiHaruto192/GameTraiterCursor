using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UpgradementUI;
    [SerializeField] private Slider maxHealthBar;
    [SerializeField] private Slider attackBar;
    [SerializeField] private Slider jumpForceBar;
    [SerializeField] private TextMeshProUGUI availableCurrency;

    [Header("Dữ liệu liên kết")]
    [SerializeField] private GameObject player;
    [Tooltip("Kéo file ScriptableObject của viên Crystal vào đây")]
    [SerializeField] private ItemData crystalData;

    private PlayerHealth health;
    private PlayerController controller;
    private InventoryManager inventoryManager; // Gọi trực tiếp túi đồ

    private void Awake()
    {
        // Vẫn giữ việc lấy Component của Player như cũ
        if (player != null)
        {
            health = player.GetComponent<PlayerHealth>();
            controller = player.GetComponent<PlayerController>();
        }
    }

    private void OnEnable()
    {
        // Mỗi lần bật bảng lên, chạy đi tìm Túi đồ trong màn chơi
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        // Cập nhật lại UI
        RefreshCurrencyUI();
    }

    private void RefreshCurrencyUI()
    {
        if (inventoryManager != null && crystalData != null)
        {
            // Bảo túi đồ đếm xem đang có bao nhiêu viên Crystal
            int currentCrystals = inventoryManager.GetItemQuantity(crystalData);
            availableCurrency.text = $"Available Currency: {currentCrystals} crystals";
        }
    }

    public void UpgradeMaxHealth()
    {
        if (inventoryManager == null || crystalData == null) return;

        int cost = 1; // Giá nâng cấp là 1 viên (bạn có thể thay đổi tùy ý)
        int currentCrystals = inventoryManager.GetItemQuantity(crystalData);

        if (currentCrystals >= cost && maxHealthBar.value < maxHealthBar.maxValue)
        {
            // 1. Trừ đồ trong túi
            inventoryManager.ConsumeItem(crystalData, cost);

            // 2. Tăng chỉ số
            maxHealthBar.value++;
            if (health != null) health.maxHealth++;

            // 3. Lưu vào kho vĩnh cửu
            if (PlayerDataManager.Instance != null && health != null)
            {
                PlayerDataManager.Instance.maxHealth = health.maxHealth;
            }
            if (health != null) health.InitHPUI();
            // 4. F5 lại chữ trên màn hình
            RefreshCurrencyUI();
        }
        else
        {
            Debug.Log("KHÔNG ĐỦ CRYSTAL HOẶC ĐÃ MAX CẤP!");
        }
    }

    public void UpgradeAttack()
    {
        if (inventoryManager == null || crystalData == null) return;

        int cost = 1;
        int currentCrystals = inventoryManager.GetItemQuantity(crystalData);

        if (currentCrystals >= cost && attackBar.value < attackBar.maxValue)
        {
            inventoryManager.ConsumeItem(crystalData, cost);

            attackBar.value++;
            if (controller != null) controller.attackDamage += 2;

            if (PlayerDataManager.Instance != null && controller != null)
            {
                PlayerDataManager.Instance.attackDamage = controller.attackDamage;
            }

            RefreshCurrencyUI();
        }
        else
        {
            Debug.Log("KHÔNG ĐỦ CRYSTAL HOẶC ĐÃ MAX CẤP!");
        }
    }

    public void UpgradeJumpforce()
    {
        if (inventoryManager == null || crystalData == null) return;

        int cost = 1;
        int currentCrystals = inventoryManager.GetItemQuantity(crystalData);

        if (currentCrystals >= cost && jumpForceBar.value < jumpForceBar.maxValue)
        {
            inventoryManager.ConsumeItem(crystalData, cost);

            jumpForceBar.value++;
            if (controller != null) controller.jumpForce += 2;

            if (PlayerDataManager.Instance != null && controller != null)
            {
                // Nếu có biến jumpForce trong PlayerDataManager thì bạn thêm vào đây
            }

            RefreshCurrencyUI();
        }
        else
        {
            Debug.Log("KHÔNG ĐỦ CRYSTAL HOẶC ĐÃ MAX CẤP!");
        }
    }

    public void Quit()
    {
        UpgradementUI.SetActive(false);
    }
}