using UnityEngine;

public class SpellAnimationEvent : MonoBehaviour
{
    [Header("Hitbox Sát Thương của Phép")]
    public Collider2D damageCollider;

    void Start()
    {
        // Đảm bảo khi cục phép vừa đẻ ra, nó chưa thể gây sát thương ngay
        // (Phải chờ animation sủi bọt/bay lên thì mới bật)
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }

    // =================================================
    // CÁC HÀM NÀY SẼ ĐƯỢC GỌI BỞI ANIMATION CỦA CÁI SPELL
    // =================================================

    // Gọi ở frame phép thuật thực sự bùng nổ / đâm lên
    public void EnableSpellDamage()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = true;
        }
    }

    // (Tùy chọn) Gọi ở frame phép thuật bắt đầu tan biến
    public void DisableSpellDamage()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }
    // Gọi hàm destroy
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}