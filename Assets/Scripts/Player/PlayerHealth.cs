using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Cài đặt Phản hồi đòn đánh")]
    public float knockbackDuration = 0.2f; // Thời gian bị khóa di chuyển để văng lùi
    public float iFrameDuration = 1.2f;    // Thời gian bất tử (không nhận sát thương)

    public bool isKnockedBack = false;     // Cờ khóa di chuyển (kết nối với file Player.cs)
    private bool isInvulnerable = false;   // Trạng thái bất tử

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {

        // 1. Nếu đang tàng hình (bất tử) thì bỏ qua không nhận damage
        if (isInvulnerable) return;

        // 2. Trừ máu
        currentHealth -= damage;
        Debug.Log("Ouch! Mất " + damage + " máu. Còn lại: " + currentHealth);

        // 3. Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 4. Bật 2 hiệu ứng: Văng lùi và Chớp nháy
            StartCoroutine(KnockbackRoutine(knockbackForce));
            StartCoroutine(IFrameRoutine());
        }
    }

    IEnumerator KnockbackRoutine(Vector2 force)
    {
        isKnockedBack = true;

        // GÁN THẲNG VẬN TỐC thay vì dùng AddForce
        // Cách này ép nhân vật phải văng đi bất chấp khối lượng (Mass) hay ma sát
        rb.linearVelocity = force;

        // Chờ hết thời gian văng lùi (0.2s)
        yield return new WaitForSeconds(knockbackDuration);

        // [TÙY CHỌN] Hãm lực ngang lại bằng 0 để nhân vật rớt xuống không bị trượt dài trên đất
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        isKnockedBack = false;
    }
    IEnumerator IFrameRoutine()
    {
        isInvulnerable = true;

        // Lặp 6 lần, mỗi lần chớp tắt (tổng thời gian = iFrameDuration)
        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f); // Mờ đi 50%
            yield return new WaitForSeconds(iFrameDuration / 12);

            spriteRenderer.color = new Color(1, 1, 1, 1f);   // Đậm lại 100%
            yield return new WaitForSeconds(iFrameDuration / 12);
        }

        isInvulnerable = false;
    }

    void Die()
    {
        Debug.Log("Player Game Over!");
        // Gọi màn hình thua hoặc Reset Scene ở đây
    }
}