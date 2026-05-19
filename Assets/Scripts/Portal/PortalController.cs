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

    [Tooltip("Đặt một cái tên độc nhất cho cổng này. VD: Cong_Rung_1")]
    public string portalID;

    private bool isUnlocked = false;
    private bool isPlayerInRange = false;
    private InventoryManager inventoryManager;

    void Start()
    {
        // Ẩn UI lúc mới bắt đầu game
        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }

        // 1. Nếu cổng không cần khóa -> Mặc định là đã mở
        if (!needKey)
        {
            isUnlocked = true;
        }
        // 2. Nếu cổng cần khóa -> Kiểm tra xem trong kho vĩnh cửu cổng này đã mở trước đó chưa
        else if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.unlockedPortals.Contains(portalID))
        {
            isUnlocked = true;
        }
    }

    void Update()
    {
        // Nếu Player đang đứng ở cổng và bấm phím F
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithPortal();
        }
    }

    private void InteractWithPortal()
    {
        // Nếu cổng đã mở thì đi luôn
        if (isUnlocked)
        {
            TeleportToNextScene();
        }
        // Nếu cổng chưa mở thì nạp chìa khóa
        else
        {
            TryUnlockPortal();
        }
    }

    private void TryUnlockPortal()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }

        if (inventoryManager != null && teleStoneData != null)
        {
            int stoneCount = inventoryManager.GetItemQuantity(teleStoneData);

            if (stoneCount >= 1)
            {
                // 1. Trừ 1 viên đá trong túi
                inventoryManager.ConsumeItem(teleStoneData, 1);

                // 2. Mở khóa cổng
                isUnlocked = true;

                // 3. Đưa tên cổng này vào danh sách đã mở của kho vĩnh cửu
                if (PlayerDataManager.Instance != null && !string.IsNullOrEmpty(portalID))
                {
                    PlayerDataManager.Instance.unlockedPortals.Add(portalID);
                }

                // 4. Đổi lại chữ trên màn hình thành Dịch chuyển
                if (interactText != null) interactText.text = "Dịch chuyển";

                // KHÔNG DỊCH CHUYỂN LUÔN NỮA, MÀ ĐỂ NGƯỜI CHƠI BẤM F LẦN 2
                Debug.Log("Đã nạp Tele Stone vào cổng! Hãy bấm F lần nữa để đi qua.");
            }
            else
            {
                Debug.Log("Cổng đã bị khóa! Bạn cần thu thập Tele Stone để mở.");
            }
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

    // ==========================================
    // NHẬN DIỆN PLAYER RA VÀO VÙNG
    // ==========================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Đổi chữ ngay khi người chơi vừa bước vào tùy theo trạng thái cổng
            if (interactText != null)
            {
                if (isUnlocked)
                {
                    interactText.text = "Dịch chuyển";
                }
                else
                {
                    interactText.text = "Cần 1 TeleStone";
                }
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