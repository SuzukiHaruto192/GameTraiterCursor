using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour // BỎ KẾ THỪA
{
    [Header("Chỉ số Máu Boss")]
    public int maxHealth = 200;
    public int currentHealth;
    private bool isDead = false;

    [Header("Hiệu ứng Chớp Đỏ")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartFlash();

        // Gửi tin nhắn để Boss bật hiệu ứng choáng/văng (nếu BossMovement có hàm OnDamageTaken)
        SendMessage("OnDamageTaken", attackerPos, SendMessageOptions.DontRequireReceiver);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StartFlash()
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
        flashCoroutine = null;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(">>> BOSS ĐÃ BỊ TIÊU DIỆT! <<<");

        // Gửi tin nhắn ngưng di chuyển
        SendMessage("DisableMovement", SendMessageOptions.DontRequireReceiver);

        BossMovement bm = GetComponent<BossMovement>();
        if (bm != null) bm.enabled = false;

        BossAttackController bac = GetComponent<BossAttackController>();
        if (bac != null) bac.enabled = false;

        // Boss thường có Animation chết cực kỳ hoành tráng, bạn có thể gọi SetTrigger("Die") ở đây.
        // Tạm thời mình giữ nguyên lệnh Destroy trễ 0.5 giây của bạn.
        Destroy(gameObject, 0.5f);
    }
}