using UnityEngine;

public class HitEffectPlayer : MonoBehaviour
{
    [Header("Gắn Prefab Particle System của vệt chém vào đây")]
    public GameObject hitEffectPrefab;

    [Header("Âm thanh khi chém trúng")]
    public AudioClip hitSound; // BIẾN MỚI ĐỂ CHỨA FILE ÂM THANH

    // Hàm này sẽ được gọi từ script tấn công khi quét trúng quái
    public void PlayHitEffect(Vector2 hitPosition)
    {
        // 1. Sinh ra hiệu ứng hình ảnh (Particle System)
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            Destroy(effect, 0.5f); // Hủy sau 0.5s
        }

        // 2. Sinh ra hiệu ứng âm thanh
        if (hitSound != null)
        {
            // Ưu tiên dùng AudioManager có sẵn của bạn để dễ đồng bộ âm lượng
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(hitSound);
            }
            else
            {
                // Phương án dự phòng: Tự phát âm thanh ngay tại điểm chém nếu không tìm thấy AudioManager
                AudioSource.PlayClipAtPoint(hitSound, hitPosition);
            }
        }
    }
}