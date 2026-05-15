using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("--- ÂM THANH SINH TỒN ---")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Hiển thị HP (Kiếm UI)")]
    public GameObject swordPrefab;     // Kéo Prefab thanh kiếm 1 máu vào đây
    public Transform swordContainer;   // Kéo object Swords_Container (có gắn Horizontal Layout Group) vào đây
    private List<GameObject> swords = new List<GameObject>(); // Danh sách quản lý kiếm tự động

    [Header("Số crystal của người chơi")]
    public int crystals;

    public bool isInvulnerable { get; private set; } = false;

    private PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        currentHealth = maxHealth;

        // Sinh ra các thanh kiếm UI ngay khi game bắt đầu
        InitHPUI();
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        // Kiểm tra né tránh từ Controller (Đỡ đòn, Đang lướt, Hoặc đang chết)
        if (controller.isInvincible || isInvulnerable) return;
        if (controller.stateMachine.CurrentState == controller.dashState) return;
        if (controller.stateMachine.CurrentState == controller.deadState) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Trúng đòn! Máu còn: " + currentHealth);

        // Hiệu ứng vỡ UI (Truyền vào currentHealth để lấy đúng vị trí thanh kiếm bị gãy)
        if (currentHealth >= 0 && currentHealth < swords.Count)
        {
            AnimateSwordBreaking(currentHealth);
        }

        if (currentHealth <= 0)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(deathSound);
            controller.stateMachine.ChangeState(controller.deadState);
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(hurtSound);

            // 1. Ép State Machine chuyển sang Hurt
            controller.stateMachine.ChangeState(controller.hurtState);

            // 2. Ép lực văng lùi vật lý
            controller.rb.linearVelocity = knockbackForce;

            // 3. Gọi dàn hiệu ứng múa may quay cuồng bên PlayerFX
            if (controller.playerFX != null)
            {
                controller.playerFX.PlayDamageEffects();
            }
        }
    }

    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
    }

    void InitHPUI()
    {
        // 1. Xóa sạch các thanh kiếm cũ (nếu có, đề phòng chơi lại màn)
        foreach (Transform child in swordContainer)
        {
            Destroy(child.gameObject);
        }
        swords.Clear();

        // 2. Sinh ra số lượng thanh kiếm tương ứng với maxHealth
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject newSword = Instantiate(swordPrefab, swordContainer);
            swords.Add(newSword);

            // Đảm bảo kiếm hiển thị sáng rõ lúc ban đầu
            Image swordImg = newSword.GetComponent<Image>();
            if (swordImg != null)
            {
                swordImg.color = Color.white;
            }
        }
    }

    void AnimateSwordBreaking(int index)
    {
        // Đảm bảo không gọi vượt quá số lượng kiếm đang có
        if (index < 0 || index >= swords.Count) return;

        GameObject sword = swords[index];
        RectTransform rect = sword.GetComponent<RectTransform>();
        Image img = sword.GetComponent<Image>();

        if (rect != null && img != null)
        {
            // Hủy các DOTween cũ đang chạy trên thanh kiếm này (nếu có)
            rect.DOKill();
            img.DOKill();

            // Hiệu ứng 1: Rung lắc mạnh thanh kiếm
            rect.DOShakeRotation(0.4f, new Vector3(0, 0, 40f), 10, 90f);

            // Hiệu ứng 2: Nháy sáng đỏ, sau đó mờ dần thành màu xám tối
            img.DOColor(Color.red, 0.1f).OnComplete(() => {
                img.DOColor(new Color(0.2f, 0.2f, 0.2f, 0.5f), 0.3f);
            });
        }
    }
}