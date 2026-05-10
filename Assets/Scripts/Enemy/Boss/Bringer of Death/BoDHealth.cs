using UnityEngine;
using System.Collections; // Thêm dòng này để dùng Coroutine nhấp nháy màu

public class BoDHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead { get; private set; } // Các script khác có thể đọc, nhưng chỉ script này được sửa

    [Header("Hiệu ứng khi bị đánh")]
    public float flashDuration = 0.1f;
    public Color damageColor = Color.white;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Xử lý Loot đồ")]
    private InventoryManager inventory;
    public ItemData dropItem;
    public GameObject lootPrefab;
    [Range(0, 5)] public int dropCount = 1;

    [Header("Cài đặt Chống Stun-lock")]
    public float hurtCooldown = 1.0f; // Khoảng cách tối thiểu giữa 2 lần bị khựng (Hurt)
    private float lastHurtTime;

    public float invincibilityDuration = 0.2f; // Thời gian bất tử ngắn sau khi trúng đòn
    private float invincibilityTimer;

    private BoDAnimation animController;

    void Start() // Bỏ 'protected override'
    {
        currentHealth = maxHealth;
        isDead = false;

        animController = GetComponent<BoDAnimation>();

        // Tìm SpriteRenderer để làm hiệu ứng nhấp nháy
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    // Bỏ chữ 'override'
    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead || invincibilityTimer > 0) return;

        // 1. TỰ TRỪ MÁU TẠI ĐÂY (Thay vì gọi base)
        currentHealth -= damage;

        // 2. HIỆU ỨNG NHẤP NHÁY
        if (spriteRenderer != null) StartCoroutine(FlashRoutine());

        // 3. KIỂM TRA CHẾT
        if (currentHealth <= 0)
        {
            Die();
            return; // Quan trọng: Chết rồi thì không chạy logic Hurt bên dưới nữa
        }

        // 4. LOGIC CHỐNG STUN-LOCK
        invincibilityTimer = invincibilityDuration;

        if (Time.time >= lastHurtTime + hurtCooldown)
        {
            if (animController != null)
            {
                animController.TriggerHurt();
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

    // Bỏ chữ 'protected override'
    private void Die()
    {
        isDead = true;
        // Tắt các script điều khiển
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
    // An