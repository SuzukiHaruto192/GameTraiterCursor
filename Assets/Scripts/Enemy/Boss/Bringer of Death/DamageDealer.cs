using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Cài đặt Sát thương")]
    public int damage = 20;           // Lượng sát thương gây ra
    public string targetTag = "Player"; // Chỉ đánh mục tiêu có Tag này

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem người bị chạm có đúng là Player không
        if (other.CompareTag(targetTag))
        {
            DealDamage(other.gameObject);
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
            playerHealth.TakeDamage(damage, knockbackDirection * 15f);
        }
    }
}