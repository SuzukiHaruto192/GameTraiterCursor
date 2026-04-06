using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerFX : MonoBehaviour
{
    [Header("Cài đặt I-Frame")]
    public float iFrameDuration = 1.2f;
    public Color damageFlashColor = Color.red;

    [Header("Cài đặt Cảm giác đánh (Game Feel)")]
    public float hitStopDuration = 0.1f;
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.3f;
    public int shakeVibrato = 10;

    private SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;

    // Khai báo nguồn phát rung của Cinemachine (Dành cho đòn đánh)
    private Cinemachine.CinemachineImpulseSource impulseSource;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        // Lấy component Impulse Source trên người Player
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }

    // ==========================================
    // --- HIỆU ỨNG KHI PLAYER NHẬN SÁT THƯƠNG ---
    // ==========================================
    public void PlayDamageEffects()
    {
        StartCoroutine(HitStopRoutine());
        StartCoroutine(IFrameRoutine());
        // Rung màn hình bằng DOTween khi bị quái đánh
        Camera.main.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90f, false, true);
    }

    private IEnumerator IFrameRoutine()
    {
        // Khóa máu bên PlayerHealth
        playerHealth.SetInvulnerable(true);

        Color originalColor = Color.white;
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.1f);

        // Nhấp nháy
        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(iFrameDuration / 12);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(iFrameDuration / 12);
        }

        // Mở khóa máu
        playerHealth.SetInvulnerable(false);
    }

    // ==========================================
    // --- HIỆU ỨNG KHI CHÉM TRÚNG QUÁI VẬT ---
    // ==========================================

    // Gọi từ PlayerController khi CheckAttackHitbox trúng đích
    public void TriggerHitStop()
    {
        StartCoroutine(HitStopRoutine());
    }

    private IEnumerator HitStopRoutine()
    {
        // Đóng băng thời gian
        Time.timeScale = 0f;
        // Chờ theo thời gian thực (không bị ảnh hưởng bởi TimeScale)
        yield return new WaitForSecondsRealtime(hitStopDuration);
        // Trả lại nhịp độ bình thường
        Time.timeScale = 1f;
    }

    // Gọi từ PlayerController khi CheckAttackHitbox trúng đích
    public void TriggerCameraShake()
    {
        if (impulseSource != null)
        {
            // Truyền lực rung từ thông số bạn đã cài đặt trên Inspector
            impulseSource.GenerateImpulse(shakeStrength);
        }
    }
}