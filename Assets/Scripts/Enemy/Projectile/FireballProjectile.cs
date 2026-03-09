using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 7f;
    public int damage = 15;
    public float knockbackPower = 4f;
    public float lifeTime = 3f; // Tự hủy sau 3 giây để tránh rác bộ nhớ

    private Vector2 moveDirection;

    public void Setup(Vector2 direction)
    {
        moveDirection = direction;
        Destroy(gameObject, lifeTime); // Lên lịch tự hủy
    }

    void Update()
    {
        // Di chuyển thẳng theo hướng đã được Setup
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu chạm trúng Player
        if (collision.CompareTag("Player"))
        {
            /*PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Hướng bật lùi dựa trên hướng bay của quả cầu lửa
                Vector2 knockbackDir = new Vector2(moveDirection.x, 0.5f).normalized;
                playerHealth.TakeDamage(damage, knockbackDir * knockbackPower);
            }
            */
            // Chạm người chơi thì nổ tung
            Destroy(gameObject);
        }
        // Nếu chạm đất/tường (Layer Ground) thì cũng nổ tung
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}