using UnityEngine;

public class HitEffectPlayer : MonoBehaviour
{
    [Header("Gắn Prefab Particle System của vệt chém vào đây")]
    public GameObject hitEffectPrefab;

    // Hàm này sẽ được gọi từ script tấn công khi quét trúng quái
    public void PlayHitEffect(Vector2 hitPosition)
    {
        if (hitEffectPrefab != null)
        {
            // Sinh ra effect tại chính xác điểm chạm
            GameObject effect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);

            // Hủy object sau 0.5 giây để tránh đầy bộ nhớ (điều chỉnh thời gian khớp với effect của bạn)
            Destroy(effect, 0.5f);
        }
    }
}