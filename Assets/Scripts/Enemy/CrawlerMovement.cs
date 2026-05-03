using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CrawlerMovement : MonoBehaviour // BỎ KẾ THỪA
{
    public float speed = 3f;
    public LayerMask groundLayer;

    [Header("Bán kính (Từ tâm đến bụng)")]
    public float radius = 0.5f;

    [Header("Độ trễ nhô ra mép vực")]
    public float edgeOffset = 0f;

    [Header("Bị Đánh (Stun)")]
    public float stunDuration = 0.5f;
    public bool canMove = true;

    private Animator anim;
    private Rigidbody2D rb;
    private Coroutine hitCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (anim != null)
        {
            anim.speed = canMove ? 1f : 0f;
        }

        if (!canMove) return;

        transform.Translate(Vector2.right * speed * Time.deltaTime);

        Vector2 pos = transform.position;
        Vector2 right = transform.right;
        Vector2 down = -transform.up;

        float rotDir = transform.localScale.x < 0 ? -1f : 1f;

        RaycastHit2D wallHit = Physics2D.Raycast(pos, right, radius + 0.1f, groundLayer);
        if (wallHit.collider != null && wallHit.collider.gameObject != gameObject)
        {
            transform.Rotate(0, 0, 90f * rotDir);
            transform.position = wallHit.point + wallHit.normal * radius;
            return;
        }

        Vector2 edgeOrigin = pos + right * edgeOffset;
        RaycastHit2D edgeHit = Physics2D.Raycast(edgeOrigin, down, radius + 0.3f, groundLayer);
        if (edgeHit.collider == null)
        {
            transform.Rotate(0, 0, -90f * rotDir);
            transform.position += (Vector3)(right * (radius + edgeOffset));
            transform.position += (Vector3)(down * (radius + edgeOffset));
            return;
        }

        RaycastHit2D groundHit = Physics2D.Raycast(pos, down, radius * 2f, groundLayer);
        if (groundHit.collider != null && groundHit.collider.gameObject != gameObject)
        {
            float angle = Mathf.Atan2(groundHit.normal.y, groundHit.normal.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = groundHit.point + groundHit.normal * radius;
        }
    }

    // --- NHẬN TÍN HIỆU TỪ ENEMY HEALTH ---
    public void OnDamageTaken(Vector2 attackerPos)
    {
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(HitReactionRoutine());
    }

    private IEnumerator HitReactionRoutine()
    {
        canMove = false;
        // Bọ dính tường chỉ đứng im chịu trận, không văng lùi
        yield return new WaitForSeconds(stunDuration);
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        this.enabled = false;
    }
}