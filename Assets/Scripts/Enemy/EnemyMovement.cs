using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundDetection;
    public float distance = 1f;
    public LayerMask groundLayer;

    public bool canMove = true;
    private bool movingRight = true;

    // Khai báo biến Rigidbody2D
    private Rigidbody2D rb;

    void Start()
    {
        // Lấy component Rigidbody2D khi game vừa chạy
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            // Nếu không được đi (vd đang đứng lại chém), set vận tốc ngang về 0
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Bắn tia kiểm tra vực sâu
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance, groundLayer);

        if (groundInfo.collider == false)
        {
            Flip();
        }
    }

    // Các xử lý liên quan đến vật lý (như di chuyển) NÊN đặt trong FixedUpdate
    void FixedUpdate()
    {
        if (!canMove) return;

        // Dùng velocity để di chuyển sẽ giúp quái tự khựng lại khi đụng tường
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