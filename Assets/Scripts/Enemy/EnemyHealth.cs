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
    private InventoryManager inventory;
    [SerializeField] private ItemData dropItem;

    // [ĐÃ SỬA] Thay vì dùng code Find, ta mở ra Inspector để kéo thả Prefab cho chắc chắn!
    [SerializeField] private GameObject lootPrefab;
    [Range(0, 5)] public int dropCount = 1;

    private bool isDead = false;
    private Collider2D enemyCollider;
    private EnemyMovementBase movementScript;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        enemyCollider = GetComponent<Collider2D>();
        movementScript = GetComponent<EnemyMovementBase>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // [ĐÃ SỬA] Dùng GetComponentInChildren vì InventoryManager là Object Con!
        if (GameManager.Instance != null)
        {
            inventory = GameManager.Instance.GetComponentInChildren<InventoryManager>();
        }
    }

    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartFlash();
        if (anim != null) anim.SetTrigger("Hurt");
        if (movementScript != null) movementScript.OnDamageTaken(attackerPos);

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

        if (movementScript != null) movementScript.enabled = false;

        EnemyMeleeAttack melee = GetComponent<EnemyMeleeAttack>();
        if (melee != null) melee.enabled = false;

        EnemyRangedAttack ranged = GetComponent<EnemyRangedAttack>();
        if (ranged != null) ranged.enabled = false;

        EnemyContactDamage contactHurt = GetComponent<EnemyContactDamage>();
        if (contactHurt != null) contactHurt.enabled = false;

        // [ĐÃ SỬA LỖI XUYÊN MAP] Ép nó thành vật thể cứng để đập vào mặt đất
        if (enemyCollider != null)
        {
            enemyCollider.isTrigger = false;
        }

        if (rb != null)
        {
            rb.gravityScale = 1f;
            // Hãm phanh ngang lại lỡ con dơi đang bay nhanh quá trôi tuột đi
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (anim != null) anim.SetBool("isDead", true);
    }

    public void OnAnimationDeadFinish()
    {
        // 1. Rơi đồ
        if (dropItem != null && lootPrefab != null && inventory != null)
        {
            for (int i = 0; i < dropCount; i++)
            {
                // Sinh ra Prefab vật phẩm
                GameObject droppedLoot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
                droppedLoot.GetComponent<SpriteRenderer>().sprite = dropItem.icon;

                // Cộng thẳng vào túi đồ (Lưu ý: Nếu bạn muốn Player nhặt chạm vào mới cộng thì phải sửa logic chỗ này)
                inventory.AddItem(dropItem);
            }
        }

        // 2. Tiêu hủy hoàn toàn
        Destroy(gameObject);
    }
}