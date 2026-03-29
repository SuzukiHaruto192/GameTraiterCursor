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

    [Header("Xử lý Cái chết & Vật phẩm rơi (Loot)")]
    public float destroyDelay = 0.2f;
    public GameObject lootPrefab;      // Kéo Prefab Pha Lê vào đây
    [Range(0, 5)] public int dropCount = 1; // Số lượng rớt ra

    private bool isDead = false;
    private Collider2D enemyCollider;
    private EnemyMovementBase movementScript;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        enemyCollider = GetComponent<Collider2D>();
        movementScript = GetComponent<EnemyMovementBase>();
    }

    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(">>> QUÁI BỊ CHÉM! Mất " + damage + " máu. Máu hiện tại còn: " + currentHealth + "/" + maxHealth);

        if (currentHealth > 0)
        {
            StartFlash();
            if (movementScript != null) movementScript.OnDamageTaken(attackerPos);
        }
        else
        {
            Die();
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

        // ---> HỆ THỐNG RƠI ĐỒ Ở ĐÂY <---
        if (lootPrefab != null)
        {
            for (int i = 0; i < dropCount; i++)
            {
                // Sinh ra viên pha lê tại vị trí của quái vật
                Instantiate(lootPrefab, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject, destroyDelay);
    }
}