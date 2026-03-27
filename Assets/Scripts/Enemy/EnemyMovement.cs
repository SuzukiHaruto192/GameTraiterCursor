using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
// Kế thừa từ EnemyMovementBase
public class EnemyMovement : EnemyMovementBase
{
    public float moveSpeed = 2f;
    public Transform groundDetection;
    public float distance = 1f;
    public LayerMask groundLayer;

    // Đã xóa biến 'canMove' ở đây vì nó đã dùng chung với Lớp Cha
    private bool movingRight = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            // Set vận tốc ngang về 0 khi bị choáng để quái không bị trượt
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance, groundLayer);
        if (groundInfo.collider == false)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

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