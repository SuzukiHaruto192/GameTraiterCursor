using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundDetection;
    public float distance = 1f;

    // Biến này cho phép script khác điều khiển việc quái có được đi hay không
    public bool canMove = true;
    private bool movingRight = true;

    void Update()
    {
        // Nếu không được phép di chuyển (ví dụ đang bận đánh), thì thoát luôn không chạy code bên dưới
        if (!canMove) return;

        Patrol();
    }

    void Patrol()
    {
        // Di chuyển tới trước
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

        // Check vực
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);

        // Check đập tường (tùy chọn, bạn có thể bắn thêm 1 tia raycast về phía trước)

        if (groundInfo.collider == false)
        {
            Flip();
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