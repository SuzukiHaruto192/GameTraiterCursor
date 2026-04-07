using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : EnemyMovementBase
{
    [Header("--- CÀI ĐẶT ĐI DẠO NGẪU NHIÊN ---")]
    public float moveSpeed = 2f;

    [Tooltip("Thời gian đứng chờ: X = Tối thiểu, Y = Tối đa (Giây)")]
    public Vector2 waitTimeRange = new Vector2(3f, 10f);

    [Tooltip("Khoảng cách mỗi lần đi: X = Ngắn nhất, Y = Dài nhất (Unit)")]
    public Vector2 moveDistanceRange = new Vector2(2f, 5f);

    [Header("--- CẢM BIẾN VỰC & TƯỜNG ---")]
    public Transform groundDetection;
    public float downDistance = 1f;   // Dò mép vực
    public float forwardDistance = 0.5f; // Dò tường
    public LayerMask groundLayer;

    private bool movingRight = true;
    private Rigidbody2D rb;
    private Animator anim;

    // Các biến phục vụ logic nội bộ
    private enum AIState { Idle, Moving }
    private AIState currentState = AIState.Idle;
    private int currentDir = 1;
    private float currentMoveTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(PatrolRoutine());
        // Đã xóa dòng StartCoroutine ở đây
    }

    // Hàm này tự động chạy MỖI KHI object được bật lên (Kể cả khi bị Chunk tắt rồi bật lại)
    void OnEnable()
    {
        // Khởi động lại bộ não tuần tra
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (!canMove) return; // Kế thừa từ Lớp cha: Bị chém là não ngừng hoạt động

        // Chỉ quét vực và tường khi ĐANG ĐI
        if (currentState == AIState.Moving)
        {
            // 1. Quét vực (Bắn tia xuống)
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, downDistance, groundLayer);

            // 2. Quét tường (Bắn tia ngang ra đằng trước)
            Vector2 forwardDir = movingRight ? Vector2.right : Vector2.left;
            RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, forwardDir, forwardDistance, groundLayer);

            // NẾU hụt chân (gặp vực) HOẶC đụng mũi (gặp tường) -> Phanh gấp!
            if (groundInfo.collider == false || wallInfo.collider != null)
            {
                ForceStopAndFlip();
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return; // Bị chém văng thì để Lớp cha lo lực AddForce

        // Nếu đang trong State Đi -> Chạy Motor. Nếu đang Idle -> Tắt Motor
        if (currentState == AIState.Moving)
        {
            rb.linearVelocity = new Vector2(currentDir * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // --- BỘ NÃO AI (MÁY TRẠNG THÁI NGẦM) ---
    IEnumerator PatrolRoutine()
    {
        while (true) // Sống là còn đi tuần
        {
            // Nếu đang bị choáng (bị chém), chờ đến khi tỉnh lại mới nghĩ tiếp
            while (!canMove) yield return null;

            // ============================================
            // PHA 1: ĐỨNG NGHỈ (IDLE)
            // ============================================
            currentState = AIState.Idle;
            anim.SetBool("isMove", false); // Gọi Animation đứng thở

            // Random thời gian đứng chơi từ 3 đến 10 giây (theo thông số Inspector)
            float waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);
            float idleTimer = 0f;

            // Đếm ngược thời gian nghỉ (vẫn phải check canMove lỡ đang nghỉ bị ăn chém)
            while (idleTimer < waitTime)
            {
                if (canMove) idleTimer += Time.deltaTime;
                yield return null;
            }

            // ============================================
            // PHA 2: CHỌN HƯỚNG VÀ DI CHUYỂN (MOVING)
            // ============================================
            currentState = AIState.Moving;
            anim.SetBool("isMove", true); // Gọi Animation đi bộ

            // Random hướng (50% quay xe, 50% đi tiếp hướng cũ)
            int randDir = Random.Range(0, 2);
            if (randDir == 0 && movingRight) Flip();
            else if (randDir == 1 && !movingRight) Flip();

            currentDir = movingRight ? 1 : -1;

            // Tính quãng đường muốn đi (Công thức: Thời gian = Quãng đường / Vận tốc)
            float moveDistance = Random.Range(moveDistanceRange.x, moveDistanceRange.y);
            float targetMoveTime = moveDistance / moveSpeed;
            currentMoveTimer = 0f;

            // Bắt đầu cuốc bộ
            while (currentMoveTimer < targetMoveTime)
            {
                if (canMove) currentMoveTimer += Time.deltaTime;

                // CÚ CHỐT: Nếu giữa đường gặp vực/tường, hàm Update() sẽ ép currentState về Idle.
                // Lúc đó ta phải break (đập vỡ) vòng lặp cuốc bộ này ngay lập tức!
                if (currentState == AIState.Idle) break;

                yield return null;
            }
        }
    }

    // Xử lý khi gặp mép vực hoặc tường
    private void ForceStopAndFlip()
    {
        currentState = AIState.Idle; // Phanh gấp
        Flip();                      // Quay mặt lại nhìn đời
        // Vòng lặp while Moving trong Coroutine sẽ tự động bị break, và AI sẽ quay về Pha 1 (Đứng nghỉ)
    }

    public void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
        currentDir = movingRight ? 1 : -1;
    }

    // Vẽ tia laser cảm biến ra Editor để bạn dễ chỉnh
    private void OnDrawGizmosSelected()
    {
        if (groundDetection != null)
        {
            // Tia vực thẳm (Màu đỏ)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * downDistance);

            // Tia check tường (Màu xanh dương)
            Gizmos.color = Color.blue;
            Vector3 forwardDir = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + forwardDir * forwardDistance);
        }
    }
}