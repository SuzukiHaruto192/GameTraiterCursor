using UnityEngine;
using UnityEngine.SceneManagement; // Bắt buộc phải có dòng này để chuyển Scene

public class PortalController : MonoBehaviour
{
    [Header("Cài đặt Cổng dịch chuyển")]
    public string sceneToLoad;         // Tên của Scene bạn muốn chuyển tới
    public GameObject interactUI;      // Kéo thả cái bảng UI "Sửa chữa" vào đây

    private bool isPlayerInRange = false;

    void Start()
    {
        // Ẩn UI lúc mới bắt đầu game
        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }
    }

    void Update()
    {
        // Nếu Player đang đứng ở cổng và bấm phím F (hoặc E, W, tuỳ bạn)
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            TeleportToNextScene();
        }
    }

    private void TeleportToNextScene()
    {
        // Có thể thêm hiệu ứng âm thanh, hạt bụi, hoặc màn hình mờ đen (Fade out) ở đây trước khi chuyển

        // Chuyển sang scene mới
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Đang dịch chuyển tới Scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("⚠️ Bạn chưa nhập tên Scene cần chuyển tới trong Inspector!");
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
            if (interactUI != null) interactUI.SetActive(true); // Hiện nút báo
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactUI != null) interactUI.SetActive(false); // Ẩn nút báo
        }
    }
}