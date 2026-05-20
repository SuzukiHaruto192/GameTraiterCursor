using UnityEngine;

public class HitEffectPlayer : MonoBehaviour
{
    [Header("Gắn Prefab Particle System của vệt chém vào đây")]
    public GameObject hitEffectPrefab;

    // Hàm này sẽ được gọi từ script tấn công khi quét trúng quái
    public void PlayHitEffect(Vector2 hitPosition)
    {
        // 1. Sinh ra hiệu ứng hình ảnh (Particle System)
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            Destroy(effect, 0.5f); // Hủy sau 0.5s
        }
    }
}