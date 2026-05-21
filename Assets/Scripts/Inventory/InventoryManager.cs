using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    // [MỚI] BIẾN NÀY SẼ GIÚP BẢNG UI TỰ ĐỘNG LẤY ĐƯỢC ĐỒ MÀ KHÔNG CẦN KÉO THẢ
    public static InventoryManager Instance;

    public List<InventoryItem> currentItems = new List<InventoryItem>();
    public InventoryUI inventoryUI;

    private bool wasUIActive = false;

    private void Awake()
    {
        // Thiết lập trạm phát sóng
        if (Instance == null)
        {
            Instance = this;
            // Không cần DontDestroyOnLoad vì file này nằm sẵn trong GameManager bất tử rồi
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        if (PlayerDataManager.Instance != null)
        {
            currentItems = new List<InventoryItem>(PlayerDataManager.Instance.savedInventory);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (inventoryUI != null)
        {
            bool isUIActive = inventoryUI.gameObject.activeInHierarchy;

            if (isUIActive && !wasUIActive)
            {
                inventoryUI.RefreshUI();
            }

            wasUIActive = isUIActive;
        }
    }

    // ==========================================
    // HÀM NHẶT ĐỒ
    // ==========================================
    public void AddItem(ItemData newItem)
    {
        if (newItem == null) return;
        bool itemExists = false;

        foreach (InventoryItem item in currentItems)
        {
            if (item.itemData == newItem)
            {
                item.quantity++;
                itemExists = true;
                break;
            }
        }

        if (!itemExists)
        {
            InventoryItem newInvItem = new InventoryItem(newItem, 1);
            currentItems.Add(newInvItem);
        }

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.savedInventory = new List<InventoryItem>(currentItems);
        }

        if (inventoryUI != null && inventoryUI.gameObject.activeInHierarchy)
        {
            inventoryUI.RefreshUI();
        }
    }

    public int GetItemQuantity(ItemData itemToCheck)
    {
        foreach (InventoryItem item in currentItems)
        {
            if (item.itemData == itemToCheck)
            {
                return item.quantity;
            }
        }
        return 0;
    }

    public void ConsumeItem(ItemData itemToConsume, int amount)
    {
        for (int i = 0; i < currentItems.Count; i++)
        {
            if (currentItems[i].itemData == itemToConsume)
            {
                currentItems[i].quantity -= amount;

                if (currentItems[i].quantity <= 0)
                {
                    currentItems.RemoveAt(i);
                }
                break;
            }
        }

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.savedInventory = new List<InventoryItem>(currentItems);
        }

        if (inventoryUI != null && inventoryUI.gameObject.activeInHierarchy)
        {
            inventoryUI.RefreshUI();
        }
    }
}