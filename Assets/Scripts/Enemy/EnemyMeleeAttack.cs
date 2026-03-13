using UnityEngine;
using System.Collections;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Cài đặt Tấn công")]
    public float attackRange = 1.5f;
    public float attackRadius = 0.8f;
    public Transform attackPoint;
    public LayerMask playerLayer;

    [Header("Sát thương & Bật lùi")]
    public int damage = 10;
    public float knockbackPower = 5f;

    [Header("Thời gian (Timing)")]
    public float windUpTime = 0.5f;
    public float attackCooldown = 1.5f;

    private EnemyMovement groundMovement;
    private bool isAttacking = false;
    public Transform player;

    void Start()
    {
        groundMovement = GetComponent<EnemyMovement>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Khi Player lọt vào tầm đánh
        if (distToPlayer <= attackRange)
        {
            // KIỂM TRA VÀ QUAY MẶT VỀ PHÍA PLAYER TRƯỚC TIÊN
            FacePlayer();

            // Sau đó mới bắt đầu gồng và chém
            StartCoroutine(PerformAttack());
        }
    }

    // --- HÀM MỚI: Tự động quay đầu nếu Player ở sau lưng ---
    void FacePlayer()
    {
        if (groundMovement == null) return;

        // Nếu Player ở bên PHẢI quái, nhưng quái đang quay mặt sang TRÁI (localScale.x < 0)
        if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            groundMovement.Flip(); // Gọi hàm Flip từ script di chuyển để đồng bộ
        }
        // Nếu Player ở bên TRÁI quái, nhưng quái đang quay mặt sang PHẢI (localScale.x > 0)
        else if (player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            groundMovement.Flip();
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (groundMovement != null) groundMovement.canMove = false;

        // 1. Khoảng trễ (Wind-up)
        yield return new WaitForSeconds(windUpTime);

        // 2. Tung đòn (Tạo vùng sát thương tại attackPoint)
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            Vector2 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;
            knockbackDirection = new Vector2(knockbackDirection.x, 0.5f).normalized;

            hitPlayer.GetComponent<PlayerHealth>().TakeDamage(damage, knockbackDirection * knockbackPower);
        }

        // 3. Thời gian nghỉ (Cooldown)
        yield return new WaitForSeconds(attackCooldown);

        if (groundMovement != null) groundMovement.canMove = true;
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (attackPoint != null)
        {
            Gizmos.color = new Color(1, 0.5f, 0);
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}