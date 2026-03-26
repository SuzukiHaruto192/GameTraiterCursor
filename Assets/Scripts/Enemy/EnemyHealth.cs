using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Hiệu ứng Chớp Đỏ (Damage Flash)")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("Hiệu ứng Đẩy Lùi (Knockback)")]
    public float knockbackForce = 3f;     // Lực đẩy lùi quái
    public float knockbackDuration = 0.1f; // Thời gian bị đẩy lùi

    [Header("Xử lý Cái chết")]
    public float destroyDelay = 0.2f;
    private bool isDead = false;

    private Collider2D enemyCollider;
    private MonoBehaviour movementScript;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        enemyCollider = GetComponent<Collider2D>();

        // Tự động tìm kịch bản di chuyển (Thay tên script nếu cần)
        movementScript = GetComponent<CrawlerMovement>();
        if (movementScript == null) movementScript = GetComponent<FlyingEnemyMovement>();
    }

    // --- ĐÃ SỬA LỖI Ở ĐÂY: Thêm tham số Vector2 attackerPos ---
    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(">>> QUÁI BỊ CHÉM! Mất " + damage + " máu. Máu hiện tại còn: " + currentHealth + "/" + maxHealth);

        if (currentHealth > 0)
        {
            StartFlash();

            // Kích hoạt hiệu ứng đẩy lùi khi bị chém
            StartCoroutine(ApplyKnockback(attackerPos));
        }
        else
        {
            Die();
        }
    }

    // Hàm thực hiện đẩy lùi
    IEnumerator ApplyKnockback(Vector2 attackerPos)
    {
        float timer = 0f;
        // Tính toán hướng đẩy: Từ vị trí người chém hướng thẳng vào quái
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPos).normalized;

        while (timer < knockbackDuration)
        {
            // Đẩy quái lùi lại
            transform.Translate(knockbackDir * knockbackForce * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void StartFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            spriteRenderer.color = originalColor;
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(">>> QUÁI [" + gameObject.name + "] ĐÃ CHẾT! <<<");

        if (enemyCollider != null) enemyCollider.enabled = false;
        if (movementScript != null) movementScript.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}