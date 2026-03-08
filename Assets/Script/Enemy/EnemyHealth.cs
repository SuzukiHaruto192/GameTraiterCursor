using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Thêm hiệu ứng chớp đỏ, giật lùi (knockback) hoặc âm thanh ở đây

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Thêm hiệu ứng nổ, rớt tiền/vật phẩm...
        Destroy(gameObject);
    }
}