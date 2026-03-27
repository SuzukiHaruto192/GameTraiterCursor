using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
// Kế thừa từ EnemyMovementBase để dùng chung hiệu ứng Stun khi bị chém
public class FlyingEnemyMovement : EnemyMovementBase
{
    [Header("Tốc độ bay")]
    [SerializeField] private float patrolSpeed = 2f; // Tốc độ bay tuần tra
    [SerializeField] private float chaseSpeed = 4f;  // Tốc độ lao vào rượt Player

    [Header("Cài đặt Lãnh thổ (AI)")]
    [SerializeField] private float chaseRadius = 5f;      // Tầm nhìn thấy Player
    [SerializeField] private float territoryRadius = 8f;  // Ranh giới ổ (đi quá sẽ bỏ cuộc)
    [SerializeField] private float patrolRange = 4f;      // Khoảng cách bay qua lại khi tuần tra

    [Header("Cài đặt Dập dềnh (Lên xuống nhẹ)")]
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    private Vector2 homePos;           // Vị trí gốc lúc mới sinh ra
    private Vector2 logicalPosition;   // Vị trí dùng để tính toán (bỏ qua độ dập dềnh)
    private int moveDirection = 1;     // Hướng bay hiện tại (1: Phải, -1: Trái)
    private float floatTimer;
    private Transform player;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isReturningHome = false; // Trạng thái đang bay về ổ

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Lưu lại vị trí ổ
        homePos = transform.position;
        logicalPosition = homePos;

        // Xác định hướng nhìn ban đầu dựa trên scale X
        moveDirection = transform.localScale.x < 0 ? -1 : 1;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;

        if (anim != null) anim.SetBool("isMove", true);

        // Tìm Player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
        if (!canMove) return; // Kế thừa từ EnemyMovementBase (đứng im khi bị chém)

        // Tính toán hiệu ứng dập dềnh không ngừng nghỉ
        floatTimer += Time.fixedDeltaTime * floatFrequency;
        float currentOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        // --- HỆ THỐNG AI ---
        bool isChasing = false;
        if (player != null)
        {
            float distToPlayer = Vector2.Distance(logicalPosition, player.position);
            float playerDistFromHome = Vector2.Distance(homePos, player.position);

            // NẾU Player trong tầm nhìn VÀ chưa thoát khỏi ranh giới
            if (distToPlayer <= chaseRadius && playerDistFromHome <= territoryRadius)
            {
                isChasing = true;
                isReturningHome = false; // Ngắt trạng thái bay về
            }
        }

        if (isChasing)
        {
            // RƯỢT ĐUỔI
            logicalPosition = Vector2.MoveTowards(logicalPosition, player.position, chaseSpeed * Time.fixedDeltaTime);
            FlipTowards(player.position.x);
        }
        else
        {
            // KIỂM TRA XEM CÓ ĐI QUÁ XA KHÔNG
            float distFromHome = Vector2.Distance(logicalPosition, homePos);

            // Nếu đi quá xa khỏi điểm tuần tra hoặc đang trong trạng thái phải về nhà
            if (distFromHome > patrolRange || isReturningHome)
            {
                isReturningHome = true;
                logicalPosition = Vector2.MoveTowards(logicalPosition, homePos, patrolSpeed * Time.fixedDeltaTime);
                FlipTowards(homePos.x);

                // Khi đã bay về tới tâm điểm nhà thì tắt trạng thái quay về
                if (Vector2.Distance(logicalPosition, homePos) <= 0.1f)
                {
                    isReturningHome = false;
                }
            }
            else
            {
                // TUẦN TRA BÌNH THƯỜNG (Bay qua lại quanh điểm gốc)
                float leftBound = homePos.x - patrolRange;
                float rightBound = homePos.x + patrolRange;

                logicalPosition.x += patrolSpeed * moveDirection * Time.fixedDeltaTime;

                // Từ từ đưa trục Y về lại bằng với ổ (trường hợp vừa rượt Player bay lên quá cao)
                logicalPosition.y = Mathf.MoveTowards(logicalPosition.y, homePos.y, patrolSpeed * Time.fixedDeltaTime);

                if (logicalPosition.x >= rightBound && moveDirection == 1)
                {
                    FlipTowards(logicalPosition.x - 1); // Ép quay trái
                }
                else if (logicalPosition.x <= leftBound && moveDirection == -1)
                {
                    FlipTowards(logicalPosition.x + 1); // Ép quay phải
                }
            }
        }

        // --- CẬP NHẬT VỊ TRÍ ---
        // Vị trí thực tế = Vị trí theo AI + Độ dập dềnh của cánh
        rb.MovePosition(new Vector2(logicalPosition.x, logicalPosition.y + currentOffset));
    }

    // Hàm lật mặt quái vật hướng về mục tiêu
    private void FlipTowards(float targetX)
    {
        float diff = targetX - logicalPosition.x;
        // Chống lỗi quay mòng mòng liên tục khi mục tiêu nằm ngay sát tâm
        if (Mathf.Abs(diff) < 0.05f) return;

        int newDir = diff > 0 ? 1 : -1;
        if (newDir != moveDirection)
        {
            moveDirection = newDir;
            Vector3 scale = transform.localScale;
            scale.x *= -1; // Lật hình
            transform.localScale = scale;
        }
    }

    // Vẽ vòng tròn ra Scene để bạn dễ dàng căn chỉnh
    private void OnDrawGizmosSelected()
    {
        Vector2 center = Application.isPlaying ? homePos : (Vector2)transform.position;

        // Vòng Vàng: Tầm nhìn (Gắn liền với quái)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Vòng Đỏ: Ranh giới lãnh thổ (Cố định ở tâm)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, territoryRadius);

        // Đường Xanh Lá: Quãng đường tuần tra ngang
        Gizmos.color = Color.green;
        Vector2 leftLine = new Vector2(center.x - patrolRange, center.y);
        Vector2 rightLine = new Vector2(center.x + patrolRange, center.y);
        Gizmos.DrawLine(leftLine, rightLine);
    }
}