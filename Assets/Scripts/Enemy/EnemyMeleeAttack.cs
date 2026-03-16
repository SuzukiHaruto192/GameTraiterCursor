using UnityEngine;
using System.Collections;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Cài đặt Tấn công")]
    public float attackRange = 1.5f;

    // --- KHAI BÁO BIẾN Ở ĐÂY ĐỂ TRÁNH LỖI CS0103 ---
    public Vector2 attackBoxSize = new Vector2(1.5f, 1f);
    public Transform attackPoint;
    public LayerMask playerLayer;

    [Header("Sát thương & Bật lùi")]
    public int damage = 10;
    public float knockbackPower = 15f;

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

        if (distToPlayer <= attackRange)
        {
            FacePlayer();
            StartCoroutine(PerformAttack());
        }
    }

    void FacePlayer()
    {
        if (groundMovement == null) return;

        if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            groundMovement.Flip();
        }
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

        // 2. Tung đòn (Tạo vùng sát thương hình chữ nhật)
        Collider2D hitPlayer = Physics2D.OverlapBox(attackPoint.position, attackBoxSize, 0f, playerLayer);

        if (hitPlayer != null)
        {
            // --- FIX LỖI VĂNG CAO TẠI ĐÂY ---
            // Xác định xem Player đang ở bên trái (-1) hay bên phải (1) của con quái
            float pushDirection = (hitPlayer.transform.position.x > transform.position.x) ? 1f : -1f;

            // Chốt cứng góc văng: X = 1 (hoặc -1), Y = 0.2 (chỉ nảy lên một chút xíu)
            Vector2 knockbackDirection = new Vector2(pushDirection, 0.2f).normalized;

            hitPlayer.GetComponent<PlayerHealth>().TakeDamage(damage, knockbackDirection * knockbackPower);
            Debug.Log("Quái chém trúng! Lực văng: " + knockbackDirection * knockbackPower);
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
            Gizmos.color = new Color(1, 0.5f, 0); // Màu cam
            Gizmos.DrawWireCube(attackPoint.position, attackBoxSize);
        }
    }
}