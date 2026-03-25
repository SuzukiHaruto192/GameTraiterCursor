using System.Collections;
using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    [Header("Knockback")]
    public float knockbackForce = 5f;
    private Rigidbody2D rb;

    [Header("Flash Effect")]
    public Color flashColor = Color.red; // Màu khi bị đánh trúng
    public float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Death")]
    public Sprite deadSprite; // Kéo thả ảnh cái xác vào đây trên Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Lưu lại màu gốc
        }
    }

    // Hàm này sẽ được gọi khi quái nhận sát thương
    public void PlayHitEffects(Vector2 attackerPosition)
    {
        // 1. Xử lý Knockback
        if (rb != null)
        {
            // Reset vận tốc trước để tránh quái bị bay mất kiểm soát nếu dính đòn liên tục
            rb.linearVelocity = Vector2.zero;

            // Tính hướng từ người đánh -> quái
            Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;

            // Thêm một chút lực hất lên trên (trục y) cho cảm giác va chạm nảy hơn
            knockbackDirection.y = 0.5f;

            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        // 2. Xử lý Nháy sáng
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor; // Đổi sang màu đỏ
            yield return new WaitForSeconds(flashDuration); // Chờ 1 chút
            spriteRenderer.color = originalColor; // Trả về màu gốc
        }
    }

    // Hàm này sẽ được gọi khi quái chết
    // Hàm này sẽ được gọi khi quái chết
    public void PlayDeathEffect()
    {
        // 1. Tắt Animator để nó không ghi đè lên hình ảnh xác chết
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.enabled = false;
        }

        // 2. Đổi ảnh sang cái xác
        if (spriteRenderer != null && deadSprite != null)
        {
            spriteRenderer.sprite = deadSprite;
        }

        // 3. Đổi Rigidbody thành Static để vô hiệu hóa trọng lực
        // Lúc này dù bên EnemyHealth có tắt Collider thì cái xác vẫn đứng im tại chỗ không bị rơi xuyên map
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
    // Thêm hàm này vào EnemyEffect.cs
    public void PlayDeathSequence()
    {
        // 1. TẮT TẤT CẢ CÁC SCRIPT NGAY LẬP TỨC 
        // Lệnh này sẽ tìm mọi file C# đang gắn trên con quái (AI, tấn công, sát thương chạm...)
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            // Tắt hết, CHỈ giữ lại đúng script EnemyEffect này để nó chạy tiếp Coroutine chết
            if (script != this)
            {
                script.enabled = false;
            }
        }

        // Bắt đầu chuỗi thời gian hấp hối
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // 2. Chờ 1 giây để quá trình văng lùi diễn ra
        // (Lúc này các script tấn công đã bị tắt ở trên, nên quái văng lùi trong trạng thái "vô hại")
        yield return new WaitForSeconds(1f);

        // 3. Bắt đầu "hóa đá" thành xác chết
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false; // Tắt animation để hiện ảnh tĩnh

        if (spriteRenderer != null && deadSprite != null)
        {
            spriteRenderer.sprite = deadSprite;
        }

        // Tắt trọng lực, ghim xác tại chỗ
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // 4. TẮT TẤT CẢ COLLIDER ĐỂ PLAYER ĐI XUYÊN QUA XÁC CHẾT
        // Dùng GetComponentsInChildren để quét sạch cả những Collider giấu trong object con (nếu có)
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in allColliders)
        {
            coll.enabled = false;
        }

        // 5. Nằm ngoan ngoãn trong 2 giây rồi tan biến
        Destroy(gameObject, 2f);
    }
}