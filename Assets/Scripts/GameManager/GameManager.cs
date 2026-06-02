using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Tham chiếu Toàn cục")]
    private Transform _player;
    public Transform player
    {
        get
        {
            if (_player == null)
            {
                GameObject pObj = GameObject.FindGameObjectWithTag("Player");
                if (pObj != null)
                    _player = pObj.transform;
            }
            return _player;
        }
    }

    // BIẾN CỜ ĐỂ PHÂN BIỆT ĐANG LOAD TỪ FILE SAVE HAY QUA CỔNG
    private bool isLoadingFromSave = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Đăng ký sự kiện: Mỗi khi load xong Scene thì tự động chạy hàm OnSceneLoaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Gỡ sự kiện khi GameManager bị hủy để tránh lỗi
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ==========================================
    // NÚT START GAME NGOÀI MENU
    // ==========================================
    public void StartNewGame()
    {
        Time.timeScale = 1f;
        _player = null;

        isLoadingFromSave = false; // Chơi mới -> Chắc chắn không phải load từ file save

        // (Tùy chọn) Gọi hàm ClearData() ở đây để xóa sạch dữ liệu cũ nếu muốn chơi lại từ đầu

        SceneManager.LoadScene("Man_1"); // Đổi tên Scene cho đúng với màn 1 của bạn nhé
    }

    // ==========================================
    // NÚT CONTINUE NGOÀI MENU (LOAD SAVE)
    // ==========================================
    public void Continue()
    {
        Time.timeScale = 1f;

        if (SaveSystem.Instance != null && PlayerDataManager.Instance != null)
        {
            bool hasSaveFile = SaveSystem.Instance.LoadGame(PlayerDataManager.Instance);

            if (hasSaveFile)
            {
                _player = null;
                isLoadingFromSave = true; // BẬT CỜ: Báo cho hệ thống biết chuẩn bị load từ File Save!

                SceneManager.LoadScene(PlayerDataManager.Instance.lastSceneName);
            }
            else
            {
                Debug.LogWarning("Không có file Save!");
            }
        }
    }

    // ==========================================
    // BỘ ĐỊNH VỊ THÔNG MINH (CHẠY SAU KHI SCENE VỪA LOAD XONG)
    // ==========================================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj == null) return;

        PlayerHealth healthScript = pObj.GetComponent<PlayerHealth>();
        if (healthScript != null)
        {
            healthScript.RestoreFullHealth();
        }

        // TRƯỜNG HỢP 1: NẾU ĐANG LOAD TỪ FILE SAVE
        if (isLoadingFromSave && PlayerDataManager.Instance != null)
        {
            // Bỏ qua Spawn Point, đặt Player về đúng tọa độ X, Y đã lưu
            pObj.transform.position = new Vector2(PlayerDataManager.Instance.lastX, PlayerDataManager.Instance.lastY);
            Debug.Log(">>> LOAD SAVE: Dịch chuyển Player tới tọa độ: " + pObj.transform.position);

            isLoadingFromSave = false; // Làm xong nhiệm vụ thì tắt cờ đi
        }
        // TRƯỜNG HỢP 2: NẾU BƯỚC QUA CỔNG HOẶC START NEW GAME
        else
        {
            GameObject spawnPointObj = GameObject.Find("Spawn Point");
            if (spawnPointObj != null)
            {
                pObj.transform.position = spawnPointObj.transform.position;
                Debug.Log(">>> CHUYỂN MAP: Dịch chuyển Player tới Spawn Point: " + spawnPointObj.transform.position);
            }
        }
    }

    public void SaveCurrentGameData()
    {
        if (SaveSystem.Instance != null && PlayerDataManager.Instance != null)
        {
            SaveSystem.Instance.SaveGame(PlayerDataManager.Instance);
        }
    }
}