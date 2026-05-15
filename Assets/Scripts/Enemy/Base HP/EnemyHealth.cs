using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Âm thanh")]
    public AudioClip hurtSound;
    public AudioClip dieSound;

    [Header("Hiệu ứng Chớp Đỏ")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("Xử lý Loot đồ")]
    private InventoryManager inventory;
    public ItemData dropItem;
    public GameObject lootPrefab;
    [Range(0, 5)] public int dropCount = 1;

    private bool isDead = false;
    private Collider2D enemyCollider;
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (GameManager.Instance != null)
        {
            inventory = GameManager.Instance.GetComponentInChildren<InventoryManager>();
        }
    }

    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        AudioManager.Instance.PlaySFX(hurtSound);
        currentHealth -= damage;
        StartFlash();
        if (anim != null) anim.SetTrigger("Hurt");

        // BÍ KÍP ĐỘC LẬP: Gửi tin nhắn "Bị đánh" đến tất cả các script đang gắn trên người nó
        SendMessage("OnDamageTaken", attackerPos, SendMessageOptions.DontRequireReceiver);
        Debug.Log("HP Left: " + currentHealth);

        if (currentHealth <= 0)
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

        // Gửi tin nhắn yêu cầu tắt bộ não di chuyển (dù là quái bay hay đi bộ)
        SendMessage("DisableMovement", SendMessageOptions.DontRequireReceiver);

        EnemyMeleeAttack melee = GetComponent<EnemyMeleeAttack>();
        if (melee != null) melee.enabled = false;

        EnemyRangedAttack ranged = GetComponent<EnemyRangedAttack>();
        if (ranged != null) ranged.enabled = false;

        EnemyContactDamage contactHurt = GetComponent<EnemyContactDamage>();
        if (contactHurt != null) contactHurt.enabled = false;

        if (enemyCollider != null) enemyCollider.isTrigger = false;

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        AudioManager.Instance.PlaySFX(dieSound);
        if (anim != null) anim.SetBool("isDead", true);
    }

    public void OnAnimationDeadFinish()
    {
        if (dropItem != null && lootPrefab != null && inventory != null)
        {
            for (int i = 0; i < dropCount; i++)
            {
                GameObject droppedLoot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
                droppedLoot.GetComponent<SpriteRenderer>().sprite = dropItem.icon;
                inventory.AddItem(dropItem);
            }
        }
        Destroy(gameObject);
    }
}