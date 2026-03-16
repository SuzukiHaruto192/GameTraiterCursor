using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Hiệu ứng Máu văng")]
    // Kéo thả BloodSpatter Prefab vào ô này ngoài Inspector
    public GameObject bloodEffectPrefab;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Hàm nhận sát thương (PlayerMovement sẽ gọi hàm này và truyền thêm hướng chém)
    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " bị chém trúng! Còn: " + currentHealth);

        // --- TẠO HIỆU ỨNG MÁU VĂNG THEO HƯỚNG ---
        // 1. Tạo hiệu ứng máu tại vị trí của quái
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);


        // 2. Set hướng văng ngược lại với hướng chém
        bloodEffect.GetComponent<BloodSpatter>()?.SetDirection(-attackDirection);

        // Gọi hiệu ứng chớp màu (Đổi sang chớp trắng cho giống Hollow Knight hơn khi trúng đòn thông thường)
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hiệu ứng chớp trắng nhanh
    IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.15f); // Thời gian chớp trắng
        spriteRenderer.color = Color.white;     // Trả lại màu gốc (Bạn nhớ reset về Color.white nếu Sprites ban đầu là màu gốc nhé)
        // Nếu Sprites ban đầu có màu đặc biệt, hãy lưu màu gốc lại rồi reset về màu đó
    }

    void Die()
    {
        Debug.Log(gameObject.name + " ĐÃ BỊ TIÊU DIỆT!");
        // Bạn có thể tạo thêm một hiệu ứng máu nổ lớn hơn ở đây trước khi Destroy
        Destroy(gameObject);
    }
}