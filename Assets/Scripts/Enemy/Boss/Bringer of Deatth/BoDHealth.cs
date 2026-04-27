using UnityEngine;

public class BoDHealth : EnemyHealthBase
{
    private BoDAnimation animController;

    [Header("Cài đặt Chống Stun-lock")]
    public float hurtCooldown = 1.0f; // Khoảng cách tối thiểu giữa 2 lần bị khựng (Hurt)
    private float lastHurtTime;

    public float invincibilityDuration = 0.2f; // Thời gian bất tử ngắn sau khi trúng đòn
    private float invincibilityTimer;

    protected override void Start()
    {
        base.Start();
        animController = GetComponent<BoDAnimation>();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public override void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead || invincibilityTimer > 0) return;

        // Trừ máu (Gọi từ lớp cha)
        base.TakeDamage(damage, attackerPos);

        // Kích hoạt thời gian bất tử để không bị nhận sát thương liên tục trong 1 frame
        invincibilityTimer = invincibilityDuration;

        // KIỂM TRA ĐIỀU KIỆN ĐỂ CHẠY HOẠT ẢNH HURT
        // Chỉ bị Hurt nếu đã qua thời gian hồi (hurtCooldown)
        if (Time.time >= lastHurtTime + hurtCooldown)
        {
            // Kiểm tra thêm: Nếu Boss đang không trong trạng thái "Super Armor" (tùy chọn)
            if (animController != null)
            {
                animController.TriggerHurt();
                lastHurtTime = Time.time;
            }
        }
    }

    protected override void Die()
    {
        isDead = true;
        if (animController != null) animController.TriggerDeath();

        // Tắt các script điều khiển
        if (GetComponent<BoDMovement>() != null) GetComponent<BoDMovement>().enabled = false;
        if (GetComponent<BoDAttack>() != null) GetComponent<BoDAttack>().enabled = false;
    }
    protected void TriggerDestroyObject() //Flame cuối của animation Death gọi
    {
        Destroy(gameObject);
    }
}