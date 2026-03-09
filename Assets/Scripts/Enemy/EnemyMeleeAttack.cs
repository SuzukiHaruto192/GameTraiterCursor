using UnityEngine;
using System.Collections; // Cần thư viện này để dùng Coroutine (delay)

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Cài đặt Tấn công")]
    public float attackRange = 1.5f;     // Tầm phát hiện để dừng lại
    public float attackRadius = 0.8f;    // Độ rộng của nhát chém (Hitbox)
    public Transform attackPoint;        // Điểm đặt lưỡi kiếm (tạo 1 object rỗng phía trước quái)
    public LayerMask playerLayer;        // Layer của Player

    [Header("Sát thương & Bật lùi")]
    public int damage = 10;
    public float knockbackPower = 5f;

    [Header("Thời gian (Timing)")]
    public float windUpTime = 0.5f;      // Thời gian "gồng" trước khi chém
    public float attackCooldown = 1.5f;  // Thời gian nghỉ giữa 2 lần chém

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

        if (distToPlayer <= attackRange)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (groundMovement != null) groundMovement.canMove = false; // Dừng lại

        // 1. Khoảng trễ (Wind-up) - Quái gồng mình
        // Thêm animation gồng ở đây
        yield return new WaitForSeconds(windUpTime);

        // 2. Tung đòn (Tạo vùng sát thương hình tròn tại mũi kiếm)
        // Thêm animation chém ở đây
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            // Tính toán hướng bật lùi (từ quái văng về phía player)
            Vector2 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;
            // Ép hướng văng xéo lên trên một chút cho đẹp
            knockbackDirection = new Vector2(knockbackDirection.x, 0.5f).normalized;

            //hitPlayer.GetComponent<PlayerHealth>().TakeDamage(damage, knockbackDirection * knockbackPower);
        }

        // 3. Thời gian nghỉ (Cooldown)
        yield return new WaitForSeconds(attackCooldown);

        if (groundMovement != null) groundMovement.canMove = true; // Đi tiếp
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn tầm đánh (Đỏ) và vùng sát thương mũi kiếm (Cam)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (attackPoint != null)
        {
            Gizmos.color = new Color(1, 0.5f, 0); // Màu cam
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}