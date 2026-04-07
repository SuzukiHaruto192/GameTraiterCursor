// --- TOÀN BỘ FILE BossMeteor.cs ---

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Đảm bảo luôn có Rigidbody2D Kinematic
public class BossMeteor : MonoBehaviour
{
    [Header("Cài đặt Đạn/Thiên thạch")]
    // Thiên thạch rơi từ trên trời thường di chuyển nhanh hơn đạn ngang (Ví dụ: 10)
    public float speed = 10f;
    public int damage = 20;        // Sát thương lớn hơn đạn thường (Ví dụ: 20)
    public float lifetime = 7f;    // Tự hủy sau 7s để đỡ nặng máy

    void Start()
    {
        // Tự động xóa vật thể sau thời gian lifetime phòng trường hợp nó bay ra ngoài bản đồ
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // VIÊN ĐÁ DI CHUYỂN:
        // Nó luôn bay "về phía trước" theo hướng mũi tên trục X của nó.
        // (Nhưng vì Boss đã lật úp nó -90 độ, nên "về phía trước" lúc này là "xuống dưới").
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    // Xử lý va chạm (Sử dụng OnTriggerEnter2D hoạt động khi 1 trong 2 là Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. VA CHẠM VỚI NGƯỜI CHƠI (PLAYER):
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Gọi hàm TakeDamage của người chơi.
                // TRUYỀN THÊM transform.position (vị trí viên đá) để người chơi biết bị đụng từ đâu mà giật lùi.
                playerHealth.TakeDamage(damage, transform.position);
            }

            Debug.Log("Thiên thạch trúng người chơi!");
            // Va chạm xong thì biến mất
            Destroy(gameObject);
        }

        // 2. VA CHẠM VỚI MẶT ĐẤT (GROUND):
        // Kiểm tra xem vật thể đụng trúng có Layer là "Ground" hoặc Tag là "Ground" không
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.CompareTag("Ground"))
        {
            Debug.Log("Thiên thạch chạm mặt đất và bị phá hủy!");

            // Va chạm đất -> Nổ và biến mất
            Destroy(gameObject);
        }
    }
}