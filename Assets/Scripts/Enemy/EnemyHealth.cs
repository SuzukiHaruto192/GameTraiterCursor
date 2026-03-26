using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 30;
    public int currentHealth; // Đổi thành public để bạn có thể nhìn thấy máu đang giảm trực tiếp trên Inspector

    [Header("Hiệu ứng Chớp Đỏ (Damage Flash)")]
    public Color flashColor = Color.red; // Màu khi bị chém (Mặc định là Đỏ)
    public float flashDuration = 0.15f;  // Thời gian chớp (Tăng lên 0.15s để nhìn rõ hơn)

    private SpriteRenderer spriteRenderer;
    private Color originalColor;         // Lưu lại màu gốc
    private Coroutine flashCoroutine;

    [Header("Xử lý Cái chết")]
    public float destroyDelay = 0.2f;
    private bool isDead = false;

    private Collider2D enemyCollider;
    private MonoBehaviour movementScript;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        // Lưu lại màu mặc định của quái (thường là màu trắng tinh Color.white)
        originalColor = spriteRenderer.color;

        enemyCollider = GetComponent<Collider2D>();

        // Tự động tìm script di chuyển để tắt khi chết
        movementScript = GetComponent<CrawlerMovement>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // --- DÒNG DEBUG KIỂM TRA MÁU ---
        Debug.Log(">>> QUÁI BỊ CHÉM! Mất " + damage + " máu. Máu hiện tại còn: " + currentHealth + "/" + maxHealth);

        if (currentHealth > 0)
        {
            StartFlash();
        }
        else
        {
            Die();
        }
    }

    // --- CƠ CHẾ CHỚP MÀU FOOLPROOF (KHÔNG BAO GIỜ LỖI) ---
    private void StartFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            spriteRenderer.color = originalColor; // Reset màu nếu chém quá nhanh
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Đổi sang màu chớp (Đỏ)
        spriteRenderer.color = flashColor;

        // Chờ 0.15 giây
        yield return new WaitForSeconds(flashDuration);

        // Trả về màu gốc
        spriteRenderer.color = originalColor;

        flashCoroutine = null;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(">>> QUÁI [" + gameObject.name + "] ĐÃ CHẾT! <<<");

        // Tắt va chạm
        if (enemyCollider != null) enemyCollider.enabled = false;

        // Tắt di chuyển
        if (movementScript != null) movementScript.enabled = false;

        // Tự hủy
        Destroy(gameObject, destroyDelay);
    }
}