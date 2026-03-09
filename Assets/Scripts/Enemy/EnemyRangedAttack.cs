using UnityEngine;
using System.Collections;

public class EnemyRangedAttack : MonoBehaviour
{
    [Header("Cài đặt Bắn")]
    public float attackRange = 5f;
    public GameObject fireballPrefab;    // Kéo thả Prefab cầu lửa vào đây
    public Transform firePoint;          // Điểm nhổ cầu lửa (thường ở miệng quái)

    [Header("Thời gian (Timing)")]
    public float windUpTime = 0.3f;
    public float attackCooldown = 2f;

    private FlyingEnemyMovement flyingMovement;
    private bool isAttacking = false;
    public Transform player;

    void Start()
    {
        flyingMovement = GetComponent<FlyingEnemyMovement>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
        {
            StartCoroutine(ShootFireball());
        }
    }

    IEnumerator ShootFireball()
    {
        isAttacking = true;
        if (flyingMovement != null) flyingMovement.canMove = false; // Khựng lại lơ lửng

        yield return new WaitForSeconds(windUpTime);

        // Tạo ra cầu lửa tại vị trí miệng quái
        if (fireballPrefab != null && firePoint != null)
        {
            // Xác định hướng bắn từ miệng tới Player
            Vector2 shootDirection = (player.position - firePoint.position).normalized;

            // Tạo quả cầu lửa
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            // Gửi thông tin hướng bay cho script Cầu lửa
            fireball.GetComponent<FireballProjectile>().Setup(shootDirection);
        }

        yield return new WaitForSeconds(attackCooldown);

        if (flyingMovement != null) flyingMovement.canMove = true; // Bay tiếp
        isAttacking = false;
    }
}