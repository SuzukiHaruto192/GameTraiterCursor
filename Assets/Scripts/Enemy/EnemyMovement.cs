using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [Header("--- CÀI ĐẶT ĐI DẠO ---")]
    public float moveSpeed = 2f;
    public Vector2 waitTimeRange = new Vector2(3f, 10f);
    public Vector2 moveDistanceRange = new Vector2(2f, 5f);

    [Header("--- CẢM BIẾN VỰC & TƯỜNG ---")]
    public Transform groundDetection;
    public float downDistance = 1f;
    public float forwardDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("--- BỊ ĐÁNH VĂNG ---")]
    public float knockbackForce = 7f;
    public float stunDuration = 0.5f;
    public bool canMove = true;

    private bool movingRight = true;
    private Rigidbody2D rb;
    private Animator anim;
    private Coroutine hitCoroutine;

    private enum AIState { Idle, Moving }
    private AIState currentState = AIState.Idle;
    private int currentDir = 1;
    private float currentMoveTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (!canMove) return;

        if (currentState == AIState.Moving)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, downDistance, groundLayer);
            Vector2 forwardDir = movingRight ? Vector2.right : Vector2.left;
            RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, forwardDir, forwardDistance, groundLayer);

            if (groundInfo.collider == false || wallInfo.collider != null)
            {
                ForceStopAndFlip();
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (currentState == AIState.Moving)
        {
            rb.linearVelocity = new Vector2(currentDir * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            while (!canMove) yield return null;

            currentState = AIState.Idle;
            anim.SetBool("isMove", false);

            float waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);
            float idleTimer = 0f;
            while (idleTimer < waitTime)
            {
                if (canMove) idleTimer += Time.deltaTime;
                yield return null;
            }

            currentState = AIState.Moving;
            anim.SetBool("isMove", true);

            int randDir = Random.Range(0, 2);
            if (randDir == 0 && movingRight) Flip();
            else if (randDir == 1 && !movingRight) Flip();

            currentDir = movingRight ? 1 : -1;

            float moveDistance = Random.Range(moveDistanceRange.x, moveDistanceRange.y);
            float targetMoveTime = moveDistance / moveSpeed;
            currentMoveTimer = 0f;

            while (currentMoveTimer < targetMoveTime)
            {
                if (canMove) currentMoveTimer += Time.deltaTime;
                if (currentState == AIState.Idle) break;
                yield return null;
            }
        }
    }

    // Hàm này tự động được gọi từ EnemyHealth nhờ lệnh SendMessage
    public void OnDamageTaken(Vector2 attackerPos)
    {
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(HitReactionRoutine(attackerPos));
    }

    private IEnumerator HitReactionRoutine(Vector2 attackerPos)
    {
        canMove = false;
        currentState = AIState.Idle; // Bị chém là đứng hình
        anim.SetBool("isMove", false);

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

    // Hàm này gọi khi quái chết
    public void DisableMovement()
    {
        canMove = false;
        this.enabled = false;
    }

    private void ForceStopAndFlip()
    {
        currentState = AIState.Idle;
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
}