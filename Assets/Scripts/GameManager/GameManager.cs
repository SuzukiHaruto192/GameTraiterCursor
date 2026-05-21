using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ==========================================
    // TRẢ LẠI ĐOẠN NÀY ĐỂ QUÁI VẬT VÀ MAP TÌM THẤY PLAYER
    // ==========================================
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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==========================================
    // NÚT CONTINUE NGOÀI MENU GỌI HÀM NÀY
    // ==========================================
    public void Continue()
    {
        Time.timeScale = 1f;

        if (SaveSystem.Instance != null && PlayerDataManager.Instance != null)
        {
            bool hasSaveFile = SaveSystem.Instance.LoadGame(PlayerDataManager.Instance);

            if (hasSaveFile)
            {
                // Reset lại định vị player để qua scene mới nó tìm lại
                _player = null;

                SceneManager.LoadScene(PlayerDataManager.Instance.lastSceneName);
                SceneManager.sceneLoaded += RestorePlayerPosition;
            }
            else
            {
                Debug.LogWarning("Không có file Save!");
            }
        }
    }

    // Chỉ dùng để đặt lại vị trí nhân vật khi Continue
    private void RestorePlayerPosition(Scene scene, LoadSceneMode mode)
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null && PlayerDataManager.Instance != null)
        {
            pObj.transform.position = new Vector2(PlayerDataManager.Instance.lastX, PlayerDataManager.Instance.lastY);
        }
        SceneManager.sceneLoaded -= RestorePlayerPosition;
    }

    // ==========================================
    // NÚT SAVE QUÁN TRỌ GỌI HÀM NÀY
    // ==========================================
    public void SaveCurrentGameData()
    {
        if (SaveSystem.Instance != null && PlayerDataManager.Instance != null)
        {
            SaveSystem.Instance.SaveGame(PlayerDataManager.Instance);
        }
    }
}