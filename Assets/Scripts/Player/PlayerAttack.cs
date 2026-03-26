using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Chỉ số Tấn công")]
    public int attackDamage = 10;          // Lượng máu trừ đi của quái
    public float attackRange = 0.5f;       // Độ to của vòng tròn sát thương

    [Header("Cảm biến (Hitbox)")]
    public Transform attackPoint;          // Vị trí vung kiếm (đặt trước mặt Player)
    public LayerMask enemyLayers;          // Chỉ chém những vật thể thuộc Layer Enemy

    // Hàm này sẽ được PlayerMovement gọi khi bấm nút X
    public void PerformAttack()
    {
        if (attackPoint == null) return;

        // Tạo ra một vòng tròn trúng đích, tóm tất cả quái vật nằm trong vòng tròn này
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Gây sát thương cho TẤT CẢ quái vật bị chém trúng
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Đã chém trúng: " + enemy.name);

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Hàm này vẽ cái vòng tròn màu đỏ ra Scene để bạn dễ căn chỉnh độ to nhỏ
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}