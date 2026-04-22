using UnityEngine;
using System.Collections;

public class EnemyHealthBase : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 100;

    // Dùng 'protected' để các script kế thừa (như BossHealth, SlimeHealth) có thể đọc được biến này
    protected int currentHealth;
    protected bool isDead = false;

    [Header("Hiệu ứng khi bị đánh")]
    public float flashDuration = 0.1f;    // Thời gian nháy đỏ
    public Color damageColor = Color.red; // Màu nháy

    private SpriteRenderer sr;
    private Color originalColor;

    // Dùng 'virtual' để các con quái khác có thể gọi lại hàm Start này hoặc viết thêm vào
    protected virtual void Start()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
    }

    // Hàm nhận sát thương (Player sẽ gọi hàm này)
    // Tích hợp luôn vị trí hitPosition để sau này bạn làm hiệu ứng giật lùi (Knockback) nếu muốn
    public virtual void TakeDamage(int damage, Vector2 hitPosition)
    {
        Debug.Log("Da gay damage");
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " mất máu! Còn: " + currentHealth);

        // Chớp đỏ khi bị chém
        if (sr != null)
        {
            StartCoroutine(FlashRedRoutine());
        }

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Nếu chưa chết, gọi hàm OnHit (để choáng, lùi lại...)
            OnHit();
        }
    }

    protected virtual IEnumerator FlashRedRoutine()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    // Hàm gọi khi bị chém nhưng chưa chết (Dành cho hiệu ứng khựng lại/Hit stun)
    protected virtual void OnHit()
    {
        // Mặc định ở Base không làm gì cả. 
        // Sau này script SlimeMovement có thể viết đè vào đây để dừng di chuyển 0.5 giây.
    }

    // Hàm Chết
    protected virtual void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " đã bị tiêu diệt!");

        // Mặc định là xóa quái vật khỏi Scene
        Destroy(gameObject);
    }
}