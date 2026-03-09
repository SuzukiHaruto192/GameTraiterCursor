using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Cài đặt Tốc độ")]
    public float patrolSpeed = 1.5f; // Tốc độ bay lượn lờ (thường chậm hơn)
    public float chaseSpeed = 3.5f;  // Tốc độ lao vào cắn người chơi

    [Header("Cài đặt Tầm nhìn & Tuần tra")]
    public float chaseRadius = 7f;   // Tầm nhìn thấy người chơi
    public float patrolDistance = 3f;// Quãng đường bay qua lại tính từ vị trí gốc
    public Transform player;

    public bool canMove = true;

    private Rigidbody2D rb;
    private Vector2 startingPosition;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Tắt trọng lực

        startingPosition = transform.position;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (!canMove || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= chaseRadius)
        {
            ChasePlayer();
        }
        else
        {
            // Kiểm tra xem quái có đang ở xa nhà không
            float distToStart = Vector2.Distance(transform.position, startingPosition);

            // Nếu bị kéo đi quá xa khỏi khu vực tuần tra -> Bay thẳng về nhà
            if (distToStart > patrolDistance)
            {
                ReturnToStart();
            }
            else
            {
                // Nếu đang ở trong khu vực nhà -> Bay qua bay lại
                Patrol();
            }
        }
    }

    void Patrol()
    {
        // Xác định giới hạn trái và phải dựa trên vị trí gốc
        float leftBound = startingPosition.x - patrolDistance;
        float rightBound = startingPosition.x + patrolDistance;

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(patrolSpeed, 0); // Bay ngang sang phải
            FlipTowards(transform.position.x + 1);     // Quay mặt sang phải

            if (transform.position.x >= rightBound)
            {
                movingRight = false; // Đụng biên phải thì quay đầu
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(-patrolSpeed, 0); // Bay ngang sang trái
            FlipTowards(transform.position.x - 1);      // Quay mặt sang trái

            if (transform.position.x <= leftBound)
            {
                movingRight = true; // Đụng biên trái thì quay đầu
            }
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        FlipTowards(player.position.x);
    }

    void ReturnToStart()
    {
        Vector2 direction = (startingPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed; // Về nhà với tốc độ thong thả
        FlipTowards(startingPosition.x);
    }

    void FlipTowards(float targetX)
    {
        if (targetX > transform.position.x && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (targetX < transform.position.x && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Vẽ trực quan trên màn hình Scene
    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn tầm nhìn (Màu vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Vẽ đường bay tuần tra (Màu xanh lá)
        Gizmos.color = Color.green;
        // Nếu game chưa chạy (startingPosition chưa được lưu) thì lấy vị trí hiện tại làm gốc
        Vector2 drawCenter = Application.isPlaying ? startingPosition : (Vector2)transform.position;
        Vector2 leftPos = new Vector2(drawCenter.x - patrolDistance, drawCenter.y);
        Vector2 rightPos = new Vector2(drawCenter.x + patrolDistance, drawCenter.y);
        Gizmos.DrawLine(leftPos, rightPos);
    }
}