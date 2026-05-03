using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FlyingEnemyMovement : MonoBehaviour // BỎ KẾ THỪA
{
    [Header("Tốc độ bay")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Cài đặt Lãnh thổ (AI)")]
    [SerializeField] private float chaseRadius = 5f;
    [SerializeField] private float territoryRadius = 8f;
    [SerializeField] private float patrolRange = 4f;

    [Header("Cài đặt Dập dềnh")]
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;

    [Header("Bị Đánh & Văng Lùi")]
    public float knockbackForce = 7f;
    public float stunDuration = 0.5f;
    public bool canMove = true;

    private Vector2 homePos;
    private Vector2 logicalPosition;
    private int moveDirection = 1;
    private float floatTimer;
    private Transform player;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isReturningHome = false;
    private Coroutine hitCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        homePos = transform.position;
        logicalPosition = homePos;

        moveDirection = transform.localScale.x < 0 ? -1 : 1;

        if (anim != null) anim.SetBool("isMove", true);

        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        floatTimer += Time.fixedDeltaTime * floatFrequency;
        float currentOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        bool isChasing = false;
        if (player != null)
        {
            float distToPlayer = Vector2.Distance(logicalPosition, player.position);
            float playerDistFromHome = Vector2.Distance(homePos, player.position);

            if (distToPlayer <= chaseRadius && playerDistFromHome <= territoryRadius)
            {
                isChasing = true;
                isReturningHome = false;
            }
        }

        if (isChasing)
        {
            logicalPosition = Vector2.MoveTowards(logicalPosition, player.position, chaseSpeed * Time.fixedDeltaTime);
            FlipTowards(player.position.x);
        }
        else
        {
            float distFromHome = Vector2.Distance(logicalPosition, homePos);

            if (distFromHome > patrolRange || isReturningHome)
            {
                isReturningHome = true;
                logicalPosition = Vector2.MoveTowards(logicalPosition, homePos, patrolSpeed * Time.fixedDeltaTime);
                FlipTowards(homePos.x);

                if (Vector2.Distance(logicalPosition, homePos) <= 0.1f)
                {
                    isReturningHome = false;
                }
            }
            else
            {
                float leftBound = homePos.x - patrolRange;
                float rightBound = homePos.x + patrolRange;

                logicalPosition.x += patrolSpeed * moveDirection * Time.fixedDeltaTime;
                logicalPosition.y = Mathf.MoveTowards(logicalPosition.y, homePos.y, patrolSpeed * Time.fixedDeltaTime);

                if (logicalPosition.x >= rightBound && moveDirection == 1)
                {
                    FlipTowards(logicalPosition.x - 1);
                }
                else if (logicalPosition.x <= leftBound && moveDirection == -1)
                {
                    FlipTowards(logicalPosition.x + 1);
                }
            }
        }

        rb.MovePosition(new Vector2(logicalPosition.x, logicalPosition.y + currentOffset));
    }

    private void FlipTowards(float targetX)
    {
        float diff = targetX - logicalPosition.x;
        if (Mathf.Abs(diff) < 0.05f) return;

        int newDir = diff > 0 ? 1 : -1;
        if (newDir != moveDirection)
        {
            moveDirection = newDir;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // --- NHẬN TÍN HIỆU BỊ CHÉM ---
    public void OnDamageTaken(Vector2 attackerPos)
    {
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(HitReactionRoutine(attackerPos));
    }

    private IEnumerator HitReactionRoutine(Vector2 attackerPos)
    {
        canMove = false;
        if (anim != null) anim.SetBool("isMove", false);

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
        logicalPosition = transform.position; // Cập nhật lại Não Bộ để không bị giật lùi
        if (anim != null) anim.SetBool("isMove", true);
    }

    public void DisableMovement()
    {
        canMove = false;
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 center = Application.isPlaying ? homePos : (Vector2)transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, territoryRadius);

        Gizmos.color = Color.green;
        Vector2 leftLine = new Vector2(center.x - patrolRange, center.y);
        Vector2 rightLine = new Vector2(center.x + patrolRange, center.y);
        Gizmos.DrawLine(leftLine, rightLine);
    }
}