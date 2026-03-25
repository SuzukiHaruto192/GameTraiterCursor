using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private EnemyEffect effect; // Khai báo biến gọi effect

    void Start()
    {
        currentHealth = maxHealth;
        effect = GetComponent<EnemyEffect>();
    }

    // Thêm tham số attackerPosition để biết bị đánh từ hướng nào
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        // Đã chết rồi thì không nhận sát thương hay bị hất văng thêm nữa
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        // Gọi hiệu ứng giật lùi và nháy đỏ (đòn cuối cùng vẫn hất văng như bình thường)
        if (effect != null)
        {
            effect.PlayHitEffects(attackerPosition);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Tắt script này đi để hoàn toàn ngắt kết nối với các đòn đánh
        this.enabled = false;

        // Gọi chuỗi hiệu ứng: Bay lùi 1s -> Đổi Sprite -> Biến mất
        if (effect != null)
        {
            effect.PlayDeathSequence();
        }
    }
}