using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Tốc độ di chuyển")]
    public float patrolSpeed = 1.5f;   // Tốc độ tuần tra
    public float chaseSpeed = 3f;      // Tốc độ đuổi bắt

    [Header("Cài đặt Lãnh thổ (Territory)")]
    [Tooltip("Khoảng cách quái bắt đầu phát hiện và đuổi theo Player")]
    public float chaseRadius = 6f;
    [Tooltip("Giới hạn tối đa tính từ Ổ (Nhà). Nếu Player ra khỏi vòng này, quái sẽ bỏ cuộc và quay về")]
    public float territoryRadius = 8f;

    [Header("Cài đặt Tuần tra (Patrol)")]
    public float waypointRadius = 3f;   // Khoảng cách từ Nhà đến các điểm tuần tra
    public float waitTime = 1.5f;        // Thời gian chờ ở mỗi điểm

    // --- ĐÂY LÀ BIẾN ĐỂ FIX LỖI VỚI SCRIPT EnemyRangedAttack CỦA BẠN ---
    public bool canMove = true;

    private Vector2 homePos;
    private Transform player;
    private Vector2 targetWaypoint;      // Điểm tuần tra hiện tại
    private float waitTimer;            // Bộ đếm thời gian chờ
    private bool isPatrolling = true;   // Trạng thái tuần tra

    void Start()
    {
        // Lưu lại vị trí lúc mới sinh ra làm "Nhà"
        homePos = transform.position;

        // Tự động tìm Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Đặt điểm tuần tra ban đầu
        SetRandomWaypoint();
        waitTimer = waitTime;
    }

    void Update()
    {
        // Nếu không có Player, hoặc bị script khác ép đứng im (canMove = false) thì ngắt không chạy code di chuyển
        if (!canMove) return;

        // Nếu mất Player (ví dụ: bị hủy), thử tìm lại
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // Đo khoảng cách
        float distToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;
        float playerDistFromHome = player != null ? Vector2.Distance(homePos, player.position) : float.MaxValue;

        // LOGIC CHUYỂN TRẠNG THÁI:
        // NẾU Player nằm trong tầm nhìn VÀ chưa ra khỏi ranh giới ổ
        if (distToPlayer <= chaseRadius && playerDistFromHome <= territoryRadius)
        {
            // Chuyển sang đuổi bắt
            isPatrolling = false;
            ChasePlayer();
        }
        else
        {
            // Chuyển sang tuần tra (hoặc bay về tổ)
            if (!isPatrolling)
            {
                ReturnHome(); // Bay về Nhà trước khi bắt đầu tuần tra
            }
            else
            {
                Patrol();
            }
        }
    }

    void ChasePlayer()
    {
        // Bay từ từ về phía vị trí của Player
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        FlipTowards(player.position.x);
    }

    void ReturnHome()
    {
        // Nếu cách xa nhà hơn 0.1f thì bay về nhà để nghỉ ngơi
        if (Vector2.Distance(transform.position, homePos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, homePos, patrolSpeed * Time.deltaTime);
            FlipTowards(homePos.x);
        }
        else
        {
            // Khi đã về đến nhà, bắt đầu tuần tra
            isPatrolling = true;
            SetRandomWaypoint();
        }
    }

    void Patrol()
    {
        // Di chuyển về phía điểm tuần tra
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint, patrolSpeed * Time.deltaTime);
        FlipTowards(targetWaypoint.x);

        // NẾU đã đến gần điểm tuần tra
        if (Vector2.Distance(transform.position, targetWaypoint) <= 0.1f)
        {
            // Bắt đầu đếm thời gian chờ
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                // Hết thời gian chờ, đặt điểm tuần tra mới và reset timer
                SetRandomWaypoint();
                waitTimer = waitTime;
            }
        }
    }

    void SetRandomWaypoint()
    {
        // Đặt điểm tuần tra là một điểm ngẫu nhiên trên một trục tuần tra
        // (Ví dụ: tuần tra ngang)
        targetWaypoint = new Vector2(homePos.x + Random.Range(-waypointRadius, waypointRadius), homePos.y);
    }

    void FlipTowards(float targetX)
    {
        // Lật mặt quay về hướng mục tiêu
        if (targetX < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (targetX > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
    }

    // Vẽ các vòng tròn ra màn hình Scene để dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        // Vòng vàng: Tầm nhìn đuổi bắt
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Vòng đỏ: Ranh giới lãnh thổ
        Gizmos.color = Color.red;
        Vector2 drawPos = Application.isPlaying ? homePos : (Vector2)transform.position;
        Gizmos.DrawWireSphere(drawPos, territoryRadius);

        // Vòng xanh lá: Vùng tuần tra
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(drawPos, waypointRadius);
    }
}