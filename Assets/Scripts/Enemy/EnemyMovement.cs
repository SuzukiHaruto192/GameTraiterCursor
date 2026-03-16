using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    [Header("Cảm biến (Sensors)")]
    public Transform wallCheck;      // Điểm dò tường
    public Transform ledgeCheck;     // Điểm dò vực/chân cầu thang
    public float wallCheckDistance = 0.5f;
    public float ledgeCheckDistance = 0.5f;
    public LayerMask groundLayer;    // Layer của Đất/Tường

    public bool canMove = true;
    private bool movingRight = true;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 1. Xác định hướng nhìn hiện tại
        Vector2 forwardDir = movingRight ? Vector2.right : Vector2.left;

        // --- 2. BẮN TIA DÒ TƯỜNG (Bắn ngang) ---
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, forwardDir, wallCheckDistance, groundLayer);
        // Vẽ tia đỏ dò tường ra Scene
        Debug.DrawRay(wallCheck.position, forwardDir * wallCheckDistance, Color.red);

        // --- 3. BẮN TIA DÒ VỰC (Bắn cắm thẳng xuống đất) ---
        RaycastHit2D ledgeHit = Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, groundLayer);
        // Vẽ tia xanh dương dò vực ra Scene
        Debug.DrawRay(ledgeCheck.position, Vector2.down * ledgeCheckDistance, Color.blue);

        // --- 4. LOGIC QUAY ĐẦU THÔNG MINH ---
        // Nếu đụng tường (wallHit có chạm) HOẶC bước hụt (ledgeHit quét vào không khí)
        if (wallHit.collider != null || ledgeHit.collider == null)
        {
            Flip();
        }

        // Cập nhật di chuyển
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
    }

    public void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}