using UnityEngine;
using System.Collections;

public class SmartChunkActivator : MonoBehaviour
{
    [Header("--- CÀI ĐẶT CHUNK ---")]
    [Tooltip("Khoảng cách (tính theo Unit) để bật/tắt nhóm quái vật này")]
    public float activationDistance = 30f;

    private Transform player;
    private bool isChunkActive = true;

    // Biến để lưu trữ GameObject con (thực thể bên trong Chunk)
    private GameObject contentHolder;

    private void Start()
    {
        // [ĐÃ TỐI ƯU] Lấy Player trực tiếp từ trạm phát sóng GameManager cực nhanh
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
        else
        {
            Debug.LogWarning("SmartChunkActivator: Không tìm thấy Player trong GameManager! Hãy kiểm tra lại file GameManager.");
        }

        // Tạo một object con rỗng tên là "Content" và ném toàn bộ quái vào đó bằng code
        CreateContentHolder();

        // Chạy vòng lặp kiểm tra khoảng cách
        StartCoroutine(CheckDistanceRoutine());
    }

    private void CreateContentHolder()
    {
        contentHolder = new GameObject("Content");
        contentHolder.transform.SetParent(this.transform);
        contentHolder.transform.localPosition = Vector3.zero;

        // Chuyển toàn bộ các object con hiện tại (Slime, Goblin...) vào trong ContentHolder
        // Dùng vòng lặp lùi để tránh lỗi mất index khi đổi Parent
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != contentHolder.transform) // Đừng tự chuyển chính nó
            {
                child.SetParent(contentHolder.transform);
            }
        }
    }

    private IEnumerator CheckDistanceRoutine()
    {
        // Quét liên tục chừng nào game còn chạy
        while (true)
        {
            if (player != null)
            {
                // Tính khoảng cách từ tâm Chunk này tới Player
                float distance = Vector2.Distance(transform.position, player.position);

                // Nếu trong tầm -> BẬT
                if (distance <= activationDistance && !isChunkActive)
                {
                    contentHolder.SetActive(true);
                    isChunkActive = true;
                }
                // Nếu ngoài tầm -> TẮT
                else if (distance > activationDistance && isChunkActive)
                {
                    contentHolder.SetActive(false);
                    isChunkActive = false;
                }
            }

            // MẸO TỐI ƯU: Chỉ quét 0.5 giây 1 lần để hệ thống nhàn rỗi, không ăn CPU
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Vẽ vòng tròn trên Editor để bạn dễ hình dung tầm hoạt động
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}