using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Chỉ số Tấn công")]
    public int attackDamage = 10;
    public float attackRange = 0.5f;

    [Header("Cảm biến (Hitbox)")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public void PerformAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // ĐÃ SỬA LỖI Ở ĐÂY: Truyền thêm transform.position (vị trí của Player) 
                // để quái biết bị chém từ hướng nào mà văng ra xa
                enemyHealth.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}