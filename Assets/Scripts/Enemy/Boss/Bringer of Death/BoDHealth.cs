using UnityEngine;
using System.Collections;

public class BoDHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead { get; private set; }

    [Header("Hiệu ứng khi bị đánh")]
    public float flashDuration = 0.1f;
    public Color damageColor = Color.white;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Cài đặt Chống Stun-lock & Phản Đòn")]
    public float hurtCooldown = 1.0f;
    private float lastHurtTime;
    public float invincibilityDuration = 0.2f;
    private float invincibilityTimer;

    [Header("Xử lý Loot đồ")]
    private InventoryManager inventory;
    public ItemData dropItem;
    public GameObject lootPrefab;
    [Range(0, 5)] public int dropCount = 1;

    private int hitCounter = 0; // ĐẾM SỐ LẦN BỊ ĐÁNH

    private BoDAnimation animController;
    private BoDAttack attackScript;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        animController = GetComponent<BoDAnimation>();
        attackScript = GetComponent<BoDAttack>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        if (GameManager.Instance != null)
        {
            inventory = GameManager.Instance.GetComponentInChildren<InventoryManager>();
        }
    }

    void Update()
    {
        if (invincibilityTimer > 0) invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead || invincibilityTimer > 0) return;

        // 1. TỰ TRỪ MÁU TẠI ĐÂY
        currentHealth -= damage;

        // 2. HIỆU ỨNG NHẤP NHÁY (Đang gồng chém vẫn sáng nhấp nháy cho biết là trúng đòn)
        if (spriteRenderer != null) StartCoroutine(FlashRoutine());

        // 3. KIỂM TRA CHẾT
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        invincibilityTimer = invincibilityDuration;

        // ====================================================
        // SUPER ARMOR: NẾU ĐANG TUNG CHIÊU THÌ KHÔNG BỊ KHỰNG!
        // ====================================================
        if (attackScript != null && attackScript.isActing)
        {
            // Thoát ngang tại đây! Không đếm hitCounter, không gọi Hurt.
            // Máu vẫn mất nhưng Boss vẫn tiếp tục bổ kiếm xuống mặt người chơi!
            return;
        }

        // ====================================================
        // 4. LOGIC CHỐNG STUN-LOCK & ĐẾM PHẢN ĐÒN
        // ====================================================
        hitCounter++;
        if (hitCounter >= 3)
        {
            hitCounter = 0; // Reset đếm
            if (attackScript != null) attackScript.TriggerCounterAttack(); // GỌI PHẢN ĐÒN
        }
        else
        {
            // Bị đánh bình thường (Dưới 3 hit)
            if (Time.time >= lastHurtTime + hurtCooldown)
            {
                if (animController != null) animController.TriggerHurt();
                lastHurtTime = Time.time;
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        if (GetComponent<BoDMovement>() != null) GetComponent<BoDMovement>().enabled = false;
        if (GetComponent<BoDAttack>() != null) GetComponent<BoDAttack>().enabled = false;
        if (animController != null) animController.TriggerDeath();
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