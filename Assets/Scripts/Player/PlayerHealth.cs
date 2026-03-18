using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Hiển thị HP (kiếm)")]
    public GameObject[] swords;
    private Vector2[] initialPositions;
    private Quaternion[] initialRotations;

    [Header("Cài đặt Phản hồi đòn đánh")]
    public float knockbackDuration = 0.2f;
    public float iFrameDuration = 1.2f;
    public float hitStopDuration = 0.1f;
    public Color damageFlashColor = Color.red;

    [Header("Cài đặt Camera Shake")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.3f;
    public int shakeVibrato = 10;

    public bool isKnockedBack = false;
    private bool isInvulnerable = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPositions = new Vector2[swords.Length];
        initialRotations = new Quaternion[swords.Length];
        for (int i = 0; i < swords.Length; i++)
        {
            initialPositions[i] = swords[i].GetComponent<RectTransform>().anchoredPosition;
            initialRotations[i] = swords[i].GetComponent<RectTransform>().rotation;
        }

        InitHPUI(); // Sửa lại tên hàm để tránh nhầm lẫn
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        if (isInvulnerable) return;

        currentHealth -= 1;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Trúng đòn! Còn lại: " + currentHealth);

        // 🔥 Gọi hiệu ứng gãy cây kiếm ở vị trí tương ứng (thay vì lặp lại toàn bộ)
        if (currentHealth >= 0 && currentHealth < swords.Length)
        {
            AnimateSwordBreaking(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Chạy đồng loạt các hiệu ứng
            StartCoroutine(HitStopRoutine()); // Khựng hình
            StartCoroutine(KnockbackRoutine(knockbackForce)); // Đẩy lùi
            StartCoroutine(IFrameRoutine()); // Nhấp nháy vô địch

            Camera.main.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90f, false, true);
            // 💡 GỢI Ý: Gọi lệnh phát Âm thanh (AudioSource.PlayOneShot) ở đây
        }
    }

    // Chỉ dùng để setup UI lúc mới vào game hoặc hồi máu
    void InitHPUI()
    {
        for (int i = 0; i < swords.Length; i++)
        {
            swords[i].SetActive(i < currentHealth);

            // Đảm bảo Alpha và vị trí reset lại bình thường (phòng khi hồi máu)
            Image swordImg = swords[i].GetComponent<Image>();
            if (swordImg != null)
            {
                swordImg.color = new Color(1, 1, 1, 1);
                swords[i].GetComponent<RectTransform>().anchoredPosition = initialPositions[i];
                swords[i].GetComponent<RectTransform>().rotation = initialRotations[i];
            }
        }
    }

    // 🔥 Hàm hiệu ứng DOTween cho UI
    void AnimateSwordBreaking(int index)
    {
        GameObject sword = swords[index];
        RectTransform rect = sword.GetComponent<RectTransform>();
        Image img = sword.GetComponent<Image>();

        if (rect != null && img != null)
        {
            // Rớt xuống 50 pixel và xoay nghiêng -30 độ
            rect.DOAnchorPosY(-50f, 0.4f).SetRelative().SetEase(Ease.InBack);
            rect.DORotate(new Vector3(0, 0, -30f), 0.4f).SetRelative();

            // Mờ dần rồi tắt hẳn
            img.DOFade(0, 0.4f).OnComplete(() => sword.SetActive(false));
        }
    }

    // 🔥 Hàm tạo Hit Stop (Khựng hình)
    IEnumerator HitStopRoutine()
    {
        Time.timeScale = 0f; // Dừng game
        // Dùng WaitForSecondsRealtime để nó không bị ảnh hưởng bởi Time.timeScale = 0
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f; // Chạy lại bình thường
    }

    IEnumerator KnockbackRoutine(Vector2 force)
    {
        isKnockedBack = true;
        rb.linearVelocity = force;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isKnockedBack = false;
    }

    IEnumerator IFrameRoutine()
    {
        isInvulnerable = true;
        Color originalColor = Color.white;
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.1f);

        // Vòng lặp nhấp nháy i-frame của bạn
        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f); // Giảm mờ thêm xíu để dễ nhìn
            yield return new WaitForSeconds(iFrameDuration / 12);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(iFrameDuration / 12);
        }

        isInvulnerable = false;
    }

    void Die()
    {
        Debug.Log("Game Over!");
        // Gọi animation chết ở đây
    }
}