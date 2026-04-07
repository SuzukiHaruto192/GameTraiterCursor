using UnityEngine;

// Kế thừa từ EnemyMovementBase
[RequireComponent(typeof(Animator))] // Bắt buộc phải có Animator
public class CrawlerMovement : EnemyMovementBase
{
    public float speed = 3f;
    public LayerMask groundLayer;

    [Header("Bán kính (Từ tâm đến bụng)")]
    public float radius = 0.5f;

    [Header("Độ trễ nhô ra mép vực")]
    public float edgeOffset = 0f;

    private Animator anim; // Khai báo biến Animator

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        // Lấy Animator Component
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- CHÌA KHÓA ANIMATION Ở ĐÂY ---
        // Nếu canMove = true (đang đi) -> Tốc độ animation là 1 (Chạy bình thường)
        // Nếu canMove = false (bị chém choáng hoặc chết) -> Tốc độ animation là 0 (Đứng hình)
        if (anim != null)
        {
            anim.speed = canMove ? 1f : 0f;
        }

        // Nếu bị đánh/choáng/chết thì không tính toán di chuyển nữa
        if (!canMove) return;

        // 1. LUÔN LUÔN TIẾN LÊN
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        Vector2 pos = transform.position;
        Vector2 right = transform.right;
        Vector2 down = -transform.up;

        float rotDir = transform.localScale.x < 0 ? -1f : 1f;

        // 2. DÒ TƯỜNG
        RaycastHit2D wallHit = Physics2D.Raycast(pos, right, radius + 0.1f, groundLayer);
        if (wallHit.collider != null && wallHit.collider.gameObject != gameObject)
        {
            transform.Rotate(0, 0, 90f * rotDir);
            transform.position = wallHit.point + wallHit.normal * radius;
            return;
        }

        // 3. DÒ VỰC 
        Vector2 edgeOrigin = pos + right * edgeOffset;
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeOrigin, down, radius + 0.3f, groundLayer);
        if (edgeHit.collider == null)
        {
            transform.Rotate(0, 0, -90f * rotDir);
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