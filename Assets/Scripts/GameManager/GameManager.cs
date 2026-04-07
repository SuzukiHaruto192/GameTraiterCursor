using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Tạo "Trạm phát sóng" Singleton
    public static GameManager Instance { get; private set; }

    [Header("Tham chiếu Toàn cục")]
    public Transform player;

    private void Awake()
    {
        // Đảm bảo chỉ có 1 GameManager tồn tại
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Khóa mục tiêu Player ĐÚNG 1 LẦN
        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null)
            {
                player = pObj.transform;
            }
        }
    }
}