using System.Collections;
using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    [Header("Knockback")]
    public float knockbackForce = 5f;
    private Rigidbody2D rb;

    [Header("Flash Effect")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Fade Out Effect")]
    public float fadeDuration = 0.5f; // Thời gian phai mờ mất bao lâu (tính bằng giây)

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Bật lùi và nháy sáng (Dùng chung cho lúc bị đánh trúng và frame đầu lúc chết)
    public void PlayHitEffects(Vector2 attackerPosition)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;
            knockbackDirection.y = 0.5f;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    // TẮT CHỨC NĂNG (Gọi ngay lúc HP <= 0)
    public void DisableActions()
    {
        // Tắt toàn bộ Script (trừ EnemyHealth và EnemyEffect)
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script != this && script != GetComponent<EnemyHealth>())
            {
                script.enabled = false;
            }
        }

        // Tắt toàn bộ Collider để không chạm gây sát thương nữa
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D coll in allColliders)
        {
            coll.enabled = false;
        }
    }

    // MỜ DẦN VÀ TIÊU HỦY (Gọi ở frame cuối Animation)
    public void StartFadeAndDestroy()
    {
        // Cố định xác chết lại để tránh bay bổng lung tung khi đang mờ
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Kích hoạt tiến trình làm mờ
        StartCoroutine(FadeAndDestroyRoutine());
    }

    private IEnumerator FadeAndDestroyRoutine()
    {
        if (spriteRenderer != null)
        {
            float elapsedTime = 0f;
            Color startColor = spriteRenderer.color;

            // Chạy vòng lặp giảm dần độ Alpha (A) của màu sắc
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                yield return null; // Đợi frame tiếp theo
            }
        }

        // Sau khi mờ hẳn (Alpha = 0) thì Destroy
        Destroy(gameObject);
    }
}