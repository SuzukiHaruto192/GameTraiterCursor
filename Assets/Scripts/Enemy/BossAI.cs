using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public Transform player;

    [Header("Range")]
    public float detectRange = 10f;
    public float closeRange = 3f;

    [Header("Patrol Advanced")]
    public float minPatrolDistance = 2f;
    public float maxPatrolDistance = 6f;
    public float patrolSpeed = 3f;

    public float minIdleTime = 0.5f;
    public float maxIdleTime = 1.5f;

    public float standTime = 0.3f;

    [Header("Dash")]
    public float dashDistance = 5f;
    public float dashDelay = 0.2f;
    public float dashSpeed = 20f;

    [Header("Fly")]
    public float flyHeight = 2f;
    public float flyDistance = 5f;
    public float flyUpTime = 0.3f;
    public float flyForwardTime = 0.4f;
    public float flyDownTime = 0.3f;

    [Header("Jump")]
    public float jumpHeight = 1f;
    public float jumpUpTime = 0.2f;
    public float jumpForwardTime = 0.3f;
    public float jumpDownTime = 0.2f;

    [Header("Dodge")]
    public float dodgeDistance = 5f;
    public float dodgeSpeed = 15f;

    [Header("Cooldown")]
    public float actionCooldown = 0.5f;

    private Vector2 startPos;
    private Rigidbody2D rb;
    private Animator anim;

    private bool isActing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPos = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        if (!isActing)
            FacePlayer();

        if (isActing) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectRange)
        {
            StartCoroutine(Patrol());
        }
        else if (dist > closeRange)
        {
            StartCoroutine(RandomAttack());
        }
        else
        {
            StartCoroutine(Dodge());
        }
    }

    // ---------------- FACE ----------------
    void FacePlayer()
    {
        float dir = player.position.x - transform.position.x;

        if (dir > 0 && transform.localScale.x < 0)
            Flip();
        else if (dir < 0 && transform.localScale.x > 0)
            Flip();
    }

    void FaceDirection(float dir)
    {
        if (dir > 0 && transform.localScale.x < 0)
            Flip();
        else if (dir < 0 && transform.localScale.x > 0)
            Flip();
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ---------------- PATROL ----------------
    IEnumerator Patrol()
    {
        isActing = true;

        // Idle → Stand
        anim.SetTrigger("ToStand");
        yield return new WaitForSeconds(standTime);

        float distance = Random.Range(minPatrolDistance, maxPatrolDistance);
        float dir = Random.value < 0.5f ? -1 : 1;

        float targetX = Mathf.Clamp(
            transform.position.x + dir * distance,
            startPos.x - maxPatrolDistance,
            startPos.x + maxPatrolDistance
        );

        FaceDirection(dir);

        anim.SetBool("isWalking", true);

        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isWalking", false);

        float idle = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idle);

        yield return new WaitForSeconds(actionCooldown);

        isActing = false;
    }

    // ---------------- RANDOM ATTACK ----------------
    IEnumerator RandomAttack()
    {
        isActing = true;

        FacePlayer();

        int rand = Random.Range(0, 3);

        if (rand == 0) yield return DashAttack();
        else if (rand == 1) yield return FlyAttack();
        else yield return JumpAttack();

        yield return Dodge();

        yield return new WaitForSeconds(actionCooldown);

        isActing = false;
    }

    // ---------------- DASH ----------------
    IEnumerator DashAttack()
    {
        rb.linearVelocity = Vector2.zero;

        FacePlayer();
        anim.SetTrigger("DashAttack");

        yield return new WaitForSeconds(dashDelay);

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        Vector2 start = transform.position;
        Vector2 end = new Vector2(start.x + dir * dashDistance, start.y);

        yield return MoveSpeed(start, end, dashSpeed);
    }

    // ---------------- FLY ----------------
    IEnumerator FlyAttack()
    {
        rb.linearVelocity = Vector2.zero;

        FacePlayer();
        anim.SetTrigger("FlyAttack");

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        Vector2 start = transform.position;
        Vector2 peak = new Vector2(start.x, start.y + flyHeight);
        Vector2 forward = new Vector2(start.x + dir * flyDistance, start.y + 1f);
        Vector2 land = new Vector2(forward.x, start.y);

        yield return MoveTime(start, peak, flyUpTime);
        yield return MoveTime(peak, forward, flyForwardTime);
        yield return MoveTime(forward, land, flyDownTime);
    }

    // ---------------- JUMP ----------------
    IEnumerator JumpAttack()
    {
        rb.linearVelocity = Vector2.zero;

        FacePlayer();
        anim.SetTrigger("JumpAttack");

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        Vector2 start = transform.position;
        Vector2 peak = new Vector2(start.x, start.y + jumpHeight);
        Vector2 forward = new Vector2(start.x + dir * flyDistance, start.y + jumpHeight);
        Vector2 land = new Vector2(forward.x, start.y);

        yield return MoveTime(start, peak, jumpUpTime);
        yield return MoveTime(peak, forward, jumpForwardTime);
        yield return MoveTime(forward, land, jumpDownTime);
    }

    // ---------------- DODGE ----------------
    IEnumerator Dodge()
    {
        rb.linearVelocity = Vector2.zero;

        FacePlayer();
        anim.SetTrigger("Dodge");

        float dir = Mathf.Sign(transform.position.x - player.position.x);

        Vector2 start = transform.position;
        Vector2 end = new Vector2(start.x + dir * dodgeDistance, start.y);

        yield return MoveSpeed(start, end, dodgeSpeed);
    }

    // ---------------- HELPER ----------------

    IEnumerator MoveSpeed(Vector2 start, Vector2 end, float speed)
    {
        while (Vector2.Distance(transform.position, end) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, end, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator MoveTime(Vector2 start, Vector2 end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector2.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }
}