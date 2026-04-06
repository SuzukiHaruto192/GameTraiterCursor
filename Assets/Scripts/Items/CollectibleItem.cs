using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Hiệu ứng nảy lúc rớt")]
    public float dropForce = 5f;

    [Header("Hiệu ứng hút nam châm")]
    public float magnetSpeed = 8f;         // Tốc độ bay về phía Player
    public float collectDistance = 0.5f;   // Khoảng cách để ăn (biến mất)

    private Rigidbody2D rb;
    private Transform playerTarget;
    private bool isMagnetizing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tạo lực nảy ngẫu nhiên (Hơi xéo xéo lên trên) để nhìn rơi tự nhiên hơn
        Vector2 dropDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
        rb.AddForce(dropDirection * dropForce, ForceMode2D.Impulse);
    }

    void Update()
    {
        // Nếu đang trong trạng thái bị hút và đã khóa mục tiêu
        if (isMagnetizing && playerTarget != null)
        {
            // Tắt trọng lực để pha lê không bị rớt xuống đất nữa
            rb.gravityScale = 0;

            // Bay từ từ về phía Player
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, magnetSpeed * Time.deltaTime);

            // Nếu bay đủ gần (chạm vào người) -> Ăn item
            if (Vector2.Distance(transform.position, playerTarget.position) < collectDistance)
            {
                Collect();
            }
        }
    }

    // Vùng từ trường (Hitbox hút)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu người chạm vào vùng từ trường là Player VÀ viên pha lê chưa bị hút
        if (collision.CompareTag("Player") && !isMagnetizing)
        {
            playerTarget = collision.transform; // Khóa mục tiêu là Player
            isMagnetizing = true;               // Kích hoạt chế độ bay
        }
    }

    void Collect()
    {
        // Gọi túi đồ của Player để tăng số lượng
        PlayerInventory inventory = playerTarget.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddCrystal(1);
        }

        // Hủy viên pha lê khỏi map
        Destroy(gameObject);
    }
}