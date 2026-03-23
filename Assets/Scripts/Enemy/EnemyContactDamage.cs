using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Cài đặt Sát thương chạm xác")]
    public int contactDamage = 10;

    // Lực văng để khoảng 15 - 20 là đẹp cho linearVelocity
    public float knockbackPower = 15f;

    // --- XỬ LÝ KHI QUÁI LÀ VẬT THỂ RẮN (Không có Is Trigger) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage(collision.gameObject);
        }
    }

    // --- XỬ LÝ KHI QUÁI LÀ VẬT THỂ XUYÊN QUA ĐƯỢC (Có Is Trigger) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DealDamage(collision.gameObject);
        }
    }

    // Hàm dùng chung để tính toán và trừ máu
    private void DealDamage(GameObject playerObj)
    {
        PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // 1. Xác định Player đang ở bên Trái (-1) hay bên Phải (1) của quái
            float pushDirection = (playerObj.transform.position.x > transform.position.x) ? 1f : -1f;

            // 2. Chốt cứng góc văng: Văng ngang và hơi nảy lên nhẹ (0.2f)
            Vector2 knockbackDirection = new Vector2(pushDirection, 0.2f).normalized;

            // 3. Gọi hàm trừ máu bên Player
            playerHealth.TakeDamage(contactDamage, knockbackDirection * knockbackPower);
        }
    }
}