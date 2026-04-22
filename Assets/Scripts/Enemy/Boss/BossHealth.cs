using UnityEngine;
using System.Collections;

// Kế thừa từ EnemyHealthBase
public class BossHealth : EnemyHealthBase
{
    // ĐÃ XÓA TOÀN BỘ CÁC BIẾN maxHealth, currentHealth, isDead... VÌ ĐÃ CÓ Ở LỚP CHA

    // Chỉ cần viết đè lại hành động lúc chết
    protected override void Die()
    {
        isDead = true; // Biến này lấy từ lớp cha
        Debug.Log(">>> BOSS ĐÃ BỊ TIÊU DIỆT! <<<");

        BossMovement bm = GetComponent<BossMovement>();
        if (bm != null) bm.enabled = false;

        BossAttackController bac = GetComponent<BossAttackController>();
        if (bac != null) bac.enabled = false;

        Destroy(gameObject, 0.5f);
    }
}