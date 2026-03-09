using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Transform player;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    // Khai báo sẵn cả 2 loại script di chuyển
    private EnemyMovement groundMovement;
    private FlyingEnemyMovement flyingMovement;

    void Start()
    {
        // Unity sẽ tự động lục tìm trên con quái xem có gắn script nào không
        // Nếu có thì lấy, nếu không có thì biến đó sẽ mang giá trị null (rỗng)
        groundMovement = GetComponent<EnemyMovement>();
        flyingMovement = GetComponent<FlyingEnemyMovement>();

        // Tự động tìm Player nếu bạn quên kéo thả vào Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        // Nếu không có Player trong Scene thì không làm gì cả để tránh lỗi
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
        {
            // Dừng di chuyển
            SetMovementState(false);
            Attack();
        }
        else
        {
            // Ngoài tầm đánh thì cho phép di chuyển tiếp
            SetMovementState(true);
        }
    }

    // Hàm phụ trợ để bật/tắt di chuyển cho gọn code
    void SetMovementState(bool canMoveState)
    {
        // Kiểm tra xem quái có phải loại đi bộ không
        if (groundMovement != null)
        {
            groundMovement.canMove = canMoveState;
        }

        // Kiểm tra xem quái có phải loại bay không
        if (flyingMovement != null)
        {
            flyingMovement.canMove = canMoveState;
        }
    }

    void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log(gameObject.name + " đang vung vũ khí tấn công!");

            // Hàm trừ máu người chơi sẽ nằm ở đây
            // player.GetComponent<PlayerHealth>().TakeDamage(10);

            lastAttackTime = Time.time;
        }
    }

    // Vẽ vòng tròn tầm đánh màu đỏ trong tab Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}