using UnityEngine;

public class CrawlerMovement : MonoBehaviour
{
    public float speed = 3f;
    public LayerMask groundLayer;

    [Header("Bán kính (Từ tâm đến bụng)")]
    public float radius = 0.5f;

    [Header("Độ trễ nhô ra mép vực")]
    [Tooltip("0 = Tới giữa bụng mới quay. Bằng Radius = Tới mũi là quay luôn.")]
    public float edgeOffset = 0f; // CHÌA KHÓA MỚI Ở ĐÂY!

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // 1. LUÔN LUÔN TIẾN LÊN
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        Vector2 pos = transform.position;
        Vector2 right = transform.right;
        Vector2 down = -transform.up;

        float rotDir = transform.localScale.x < 0 ? -1f : 1f;

        // 2. DÒ TƯỜNG (Húc vách - Vẫn dùng mũi để đụng là leo ngay)
        RaycastHit2D wallHit = Physics2D.Raycast(pos, right, radius + 0.1f, groundLayer);

        if (wallHit.collider != null && wallHit.collider.gameObject != gameObject)
        {
            transform.Rotate(0, 0, 90f * rotDir);
            transform.position = wallHit.point + wallHit.normal * radius;
            return;
        }

        // 3. DÒ VỰC (Đã thay đổi để cho phép nhô thân ra ngoài)
        // Thay vì luôn bắn từ mũi (radius), giờ tia laser sẽ bắn theo edgeOffset
        Vector2 edgeOrigin = pos + right * edgeOffset;
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeOrigin, down, radius + 0.3f, groundLayer);

        if (edgeHit.collider == null)
        {
            transform.Rotate(0, 0, -90f * rotDir);

            // Công thức Toán Học Hoàn Hảo tự động tính toán bước nhảy cua góc dựa theo độ trễ
            transform.position += (Vector3)(right * (radius + edgeOffset));
            transform.position += (Vector3)(down * (radius + edgeOffset));
            return;
        }

        // 4. HÚT ĐẤT & CÂN BẰNG
        RaycastHit2D groundHit = Physics2D.Raycast(pos, down, radius * 2f, groundLayer);

        if (groundHit.collider != null && groundHit.collider.gameObject != gameObject)
        {
            float angle = Mathf.Atan2(groundHit.normal.y, groundHit.normal.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            transform.position = groundHit.point + groundHit.normal * radius;
        }
    }
}