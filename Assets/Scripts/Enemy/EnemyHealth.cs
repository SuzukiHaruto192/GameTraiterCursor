using UnityEngine;
using System.Collections;

// Kế thừa từ EnemyHealthBase
public class EnemyHealth : EnemyHealthBase
{
    // ĐÃ XÓA CÁC BIẾN MÁU VÀ CHỚP ĐỎ. CHỈ GIỮ LẠI CÁC BIẾN SAU:
    [Header("Xử lý Cái chết & Vật phẩm rơi (Loot)")]
    private InventoryManager inventory;
    [SerializeField] private ItemData dropItem;
    [SerializeField] private GameObject lootPrefab;
    [Range(0, 5)] public int dropCount = 1;

    private Collider2D enemyCollider;
    private EnemyMovementBase movementScript;
    private Animator anim;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start(); // Gọi lớp cha để gán máu và màu sắc

        enemyCollider = GetComponent<Collider2D>();
        movementScript = GetComponent<EnemyMovementBase>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (GameManager.Instance != null)
        {
            inventory = GameManager.Instance.GetComponentInChildren<InventoryManager>();
        }
    }

    public override void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        base.TakeDamage(damage, attackerPos); // Nhờ lớp cha trừ máu và chớp đỏ

        if (!isDead)
        {
            if (anim != null) anim.SetTrigger("Hurt");
            if (movementScript != null) movementScript.OnDamageTaken(attackerPos);
        }
    }

    protected override void Die()
    {
        isDead = true;

        if (movementScript != null) movementScript.enabled = false;

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