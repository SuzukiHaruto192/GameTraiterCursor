using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PortalController : MonoBehaviour
{
    [Header("Cài đặt Cổng dịch chuyển")]
    public string sceneToLoad;
    public GameObject interactUI;

    [Tooltip("Kéo object Text (TMP) ở bên trong InteractCanvas vào đây")]
    public TextMeshProUGUI interactText;

    [Header("Cơ chế Khóa (Tele Stone)")]
    public bool needKey = false;
    public ItemData teleStoneData;

    [Tooltip("Đặt một cái tên độc nhất cho cổng này. VD: Cong_Man_1")]
    public string portalID;

    private bool isUnlocked = false;
    private bool isPlayerInRange = false;
    private InventoryManager inventoryManager;

    void Awake()
    {
        if (interactUI != null) interactUI.SetActive(false);

        // 1. Nếu cổng mặc định mở
        if (!needKey)
        {
            isUnlocked = true;
        }
        // 2. Nếu cổng cần sửa: Kiểm tra xem trong bộ nhớ vĩnh cửu đã mở chưa
        else if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.isMan1PortalUnlocked)
        {
            isUnlocked = true;
            MoveSpawnPointToPortal(); // Tự động dời Spawn Point về đây nếu cổng đã mở trước đó!
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithPortal();
        }
    }

    private void InteractWithPortal()
    {
        if (isUnlocked)
        {
            TeleportToNextScene();
        }
        else
        {
            TryUnlockPortal();
        }
    }

    private void TryUnlockPortal()
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (inventoryManager != null && teleStoneData != null)
        {
            int stoneCount = inventoryManager.GetItemQuantity(teleStoneData);

            if (stoneCount >= 1)
            {
                // Trừ vật phẩm sửa cổng rớt ra từ Boss
                inventoryManager.ConsumeItem(teleStoneData, 1);

                isUnlocked = true;

                // Ghi nhớ vĩnh viễn vào hệ thống dữ liệu: Cổng màn 1 ĐÃ ĐƯỢC SỬA THÀNH CÔNG
                if (PlayerDataManager.Instance != null)
                {
                    PlayerDataManager.Instance.isMan1PortalUnlocked = true;
                }

                // DỜI SPAWN POINT VỀ ĐÂY NGAY LẬP TỨC!
                MoveSpawnPointToPortal();

                if (interactText != null) interactText.text = "Dịch chuyển";
                Debug.Log(">>> SỬA CỔNG THÀNH CÔNG! Spawn Point đã được dời về Cổng Dịch Chuyển.");
            }
            else
            {
                Debug.Log("Cổng đã bị hỏng! Bạn cần vật phẩm sửa cổng từ Boss để kích hoạt.");
            }
        }
    }

    // Hàm phụ trách bứng cái Spawn Point về vị trí của Cổng
    private void MoveSpawnPointToPortal()
    {
        GameObject spawnPointObj = GameObject.Find("Spawn Point");
        if (spawnPointObj != null)
        {
            // Ép tọa độ của Spawn Point trùng với tọa độ của cái Cổng này
            spawnPointObj.transform.position = transform.position;
            Debug.Log(">>> Đã dời vị trí Spawn Point về Cổng!");
        }
    }

    private void TeleportToNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Đang dịch chuyển tới Scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (interactText != null)
            {
                interactText.text = isUnlocked ? "Dịch chuyển" : "Sửa Cổng (Cần Vật Phẩm)";
            }

            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}