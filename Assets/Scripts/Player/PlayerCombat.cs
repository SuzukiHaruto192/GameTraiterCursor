using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    // --- ĐÃ GỘP CODE CỦA BẠN VÀ BẠN CỦA BẠN VÀO LÀM 1 ---
    public float attackRange = 0.5f;
    public int attackDamage = 100; // Chuyển sang int để khớp với máu quái
    public HitEffectPlayer effectController; // Code hiệu ứng của bạn bạn

    private PlayerMovement playerMove; // Code di chuyển của bạn
    public bool isInvincible = false;

    Rigidbody2D rb;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Tấn công
        if (Input.GetAxis("Vertical") < 0 && !playerMove.isGrounded && Input.GetKeyDown(KeyCode.J))     //Tấn công trên không --> Đè S và Ấn J
        {
            PerformAirSpecialSkill();
        }
        else if (Input.GetKeyDown(KeyCode.J))                                                           // Tấn công bình thường --> Ấn J
        {
            Attack();
        }

        // Đỡ đòn --> Ấn S hoặc mũi tên xuống
        if (Input.GetAxis("Vertical") < 0 && playerMove.isGrounded)
        {
            rb.linearVelocity = Vector3.zero;
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 exactHitPoint = enemy.ClosestPoint(attackPoint.position);

            // Chạy hiệu ứng đánh trúng (Code của bạn bạn)
            if (effectController != null)
            {
                effectController.PlayHitEffect(exactHitPoint);
            }

            // --- ĐÃ FIX LỖI THIẾU BIẾN Ở ĐÂY ---
            // Truyền thêm vị trí transform.position để kích hoạt hiệu ứng đẩy lùi (Knockback) cho quái
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    void PerformAirSpecialSkill()
    {

    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}