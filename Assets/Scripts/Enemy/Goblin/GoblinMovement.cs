using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class GoblinMovement : MonoBehaviour
{
    [Header("--- TỐC ĐỘ ---")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("--- TẦM NHÌN (AI) ---")]
    public float chaseRadius = 6f;    // Bán kính phát hiện Player
    public float attackRange = 1.5f;  // Khoảng cách dừng lại để chém

    [Header("--- CẢM BIẾN VỰC & TƯỜNG ---")]
    public Transform groundDetection;
    public float downDistance = 1f;
    public float forwardDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("--- ĐI DẠO ---")]
    public Vector2 waitTimeRange = new Vector2(2f, 5f);
    public Vector2 moveDistanceRange = new Vector2(2f, 5f);

    [Header("--- BỊ ĐÁNH VĂNG ---")]
    public float knockbackForce = 7f;
    public float stunDuration = 0.5f;

    public bool canMove = true;
    public bool isChasing = false;
    private bool movingRight = true;
    private int currentDir = 1;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private Coroutine hitCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (!canMove) return;

        // 1. RADAR PHÁT HIỆN PLAYER
        if (player != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            // Nằm trong vùng rượt, nhưng chưa đủ gần để chém -> RƯỢT!
            if (distToPlayer <= chaseRadius && distToPlayer > attackRange)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false; // Ngoài tầm hoặc đã vào tầm chém -> Dừng rượt
            }
        }

        // 2. CẢM BIẾN VỰC/TƯỜNG
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, downDistance, groundLayer);
        Vector2 forwardDir = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, forwardDir, forwardDistance, groundLayer);

        if (groundInfo.collider == false || wallInfo.collider != null)
        {
            if (isChasing)
            {
                // Đang rượt mà gặp vực -> Đứng lại sát mép vực nhìn
                StopMovingAnimation();
            }
            else
            {
                // Đang đi dạo mà gặp vực -> Quay đầu
                ForceStopAndFlip();
            }
        }
        else if (isChasing)
        {
            // ĐƯỜNG TRỐNG -> RƯỢT ĐUỔI
            FacePlayer();
            anim.SetBool("isMove", true);
            rb.linearVelocity = new Vector2(currentDir * chaseSpeed, rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        if (!canMove || isChasing) return; // Nếu đang rượt thì Update lo, FixedUpdate nghỉ

        // DI CHUYỂN LÚC ĐI DẠO
        if (anim.GetBool("isMove"))
        {
            rb.linearVelocity = new Vector2(currentDir * patrolSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // --- LOGIC ĐI DẠO KHI KHÔNG THẤY PLAYER ---
    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            while (!canMove || isChasing) yield return null;

            anim.SetBool("isMove", false);
            float waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);
            float timer = 0;
            while (timer < waitTime)
            {
                if (canMove && !isChasing) timer += Time.deltaTime;
                if (isChasing) break; // Đang đứng chơi mà thấy Player -> Bỏ nghỉ, đi rượt
                yield return null;
            }

            if (isChasing) continue;

            anim.SetBool("isMove", true);
            int randDir = Random.Range(0, 2);
            if (randDir == 0 && movingRight) Flip();
            else if (randDir == 1 && !movingRight) Flip();
            currentDir = movingRight ? 1 : -1;

            float moveDist = Random.Range(moveDistanceRange.x, moveDistanceRange.y);
            float moveTime = moveDist / patrolSpeed;
            timer = 0;
            while (timer < moveTime)
            {
                if (canMove && !isChasing) timer += Time.deltaTime;
                if (isChasing || !anim.GetBool("isMove")) break;
                yield return null;
            }
        }
    }

    public void StopMovingAnimation()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetBool("isMove", false);
    }

    public void FacePlayer()
    {
        if (player == null) return;
        float direction = player.position.x - transform.position.x;
        if (direction > 0 && !movingRight) Flip();
        else if (direction < 0 && movingRight) Flip();
    }

    private void ForceStopAndFlip()
    {
        StopMovingAnimation();
        Flip();
    }

    public void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
        currentDir = movingRight ? 1 : -1;
    }

    // --- BỊ ĐÁNH VĂNG ---
    public void OnDamageTaken(Vector2 attackerPos)
    {
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(HitReactionRoutine(attackerPos));
    }

    private IEnumerator HitReactionRoutine(Vector2 attackerPos)
    {
        canMove = false;
        StopMovingAnimation();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 knockbackDir = ((Vector2)transform.position - attackerPos).normalized;
            knockbackDir.y = 0.5f;
            rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(stunDuration);

        if (rb != null) rb.linearVelocity = Vector2.zero;
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        if (groundDetection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * downDistance);
        }
    }
}