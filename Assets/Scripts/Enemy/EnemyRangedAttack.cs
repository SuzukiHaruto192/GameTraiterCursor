using UnityEngine;
using System.Collections;

public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Cài đặt Bắn")]
    public float attackRange = 5f;
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("Thời gian (Timing)")]
    public float windUpTime = 0.3f;
    public float attackCooldown = 2f;

    private FlyingEnemyMovement flyingMovement;
    private Animator anim; // Thêm biến để gọi Animation
    private bool isAttacking = false;
    public Transform player;

    void Start()
    {
        flyingMovement = GetComponent<FlyingEnemyMovement>();
        anim = GetComponent<Animator>();

        // CÁCH MỚI
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
        {
            // Quay mặt về phía người chơi trước khi khạc lửa
            FacePlayer();
            StartCoroutine(ShootFireball());
        }
    }

    // --- HÀM QUAY MẶT VỀ PHÍA PLAYER ---
    void FacePlayer()
    {
        float direction = player.position.x - transform.position.x;
        Vector3 currentScale = transform.localScale;

        // Nếu Player ở bên PHẢI và Quái đang quay mặt TRÁI, lật lại
        if (direction > 0 && currentScale.x < 0)
        {
            currentScale.x *= -1;
        }
        // Nếu Player ở bên TRÁI và Quái đang quay mặt PHẢI, lật lại
        else if (direction < 0 && currentScale.x > 0)
        {
            currentScale.x *= -1;
        }
        transform.localScale = currentScale;
    }

    IEnumerator ShootFireball()
    {
        isAttacking = true;

        // 1. Dừng bay lơ lửng tại chỗ
        if (flyingMovement != null) flyingMovement.canMove = false;

        // 2. Kích hoạt Animation tấn công (Nếu bạn đã làm animation này)
        if (anim != null) anim.SetTrigger("Attack"); // Nhớ tạo trigger "Attack" trong Animator nhé!

        // 3. Thời gian gồng (Wind-up)
        yield return new WaitForSeconds(windUpTime);

        // 4. Nhổ cầu lửa
        if (fireballPrefab != null && firePoint != null)
        {
            // Tính hướng từ miệng tới Player
            Vector2 shootDirection = (player.position - firePoint.position).normalized;

            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            // Tìm script FireballProjectile trên viên đạn và truyền hướng bay vào
            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
            if (projectile != null)
            {
                projectile.Setup(shootDirection);
            }
        }

        // 5. Chờ cooldown
        yield return new WaitForSeconds(attackCooldown);

        // 6. Cho phép bay tiếp
        if (flyingMovement != null) flyingMovement.canMove = true;
        isAttacking = false;
    }

    // Vẽ vòng tròn tầm bắn màu đỏ trên Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}