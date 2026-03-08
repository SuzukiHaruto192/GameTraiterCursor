using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player; // Kéo thả Player vào đây
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    // Khai báo một biến chứa script di chuyển
    private EnemyMovement movementScript;

    void Start()
    {
        // Tự động tìm và lấy script EnemyMovement đang gắn trên cùng một con quái
        movementScript = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
        {
            // Dừng di chuyển
            if (movementScript != null)
            {
                movementScript.canMove = false;
            }

            Attack();
        }
        else
        {
            // Ngoài tầm đánh thì cho phép đi dạo tiếp
            if (movementScript != null)
            {
                movementScript.canMove = true;
            }
        }
    }

    void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Vung kiếm chém!");

            // Code kích hoạt Animation chém hoặc gọi hàm trừ máu người chơi sẽ nằm ở đây

            lastAttackTime = Time.time;
        }
    }
}