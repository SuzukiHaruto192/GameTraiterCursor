using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int currentHealth;
    public int maxHealth; // Để public để CharacterUpgradeUIManager có thể truy cập nâng cấp

    [Header("Hiển thị HP (Kiếm UI)")]
    public GameObject swordPrefab;     // Kéo Prefab thanh kiếm 1 máu vào đây
    public Transform swordContainer;   // Kéo object Swords_Container (có gắn Horizontal Layout Group) vào đây
    private List<GameObject> swords = new List<GameObject>(); // Danh sách quản lý kiếm tự động

    [Header("--- CƠ CHẾ HÚT MÁU (COMBO) ---")]
    public int hitsToHeal = 10;       // Số hit cần để hồi 1 máu
    private int currentHitCount = 0;  // Biến lưu số hit hiện tại

    // ==========================================
    // HÀM TÍCH ĐIỂM (GỌI KHI CHÉM TRÚNG QUÁI)
    // ==========================================
    public void AddHit()
    {
        // Nếu máu đã đầy thì không cần tích điểm
        if (currentHealth >= maxHealth) return;

        currentHitCount++;
        Debug.Log("Chém trúng! Tích điểm hồi máu: " + currentHitCount + "/" + hitsToHeal);

        if (currentHitCount >= hitsToHeal)
        {
            Heal(1);
            currentHitCount = 0; // Đủ 10 hit thì hồi máu xong reset lại từ số 0
        }
    }

    // ==========================================
    // HÀM HỒI MÁU VÀ CẬP NHẬT GIAO DIỆN KIẾM
    // ==========================================
    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(">>> Đã hồi máu! Máu hiện tại: " + currentHealth);

        int index = currentHealth - 1;

        // ĐÃ SỬA THÀNH .Count CHO LIST VÀ BỎ SET VỊ TRÍ THỦ CÔNG
        if (index >= 0 && index < swords.Count)
        {
            GameObject sword = swords[index];
            if (sword != null)
            {
                Image img = sword.GetComponent<Image>();
                RectTransform rect = sword.GetComponent<RectTransform>();

                if (rect != null && img != null)
                {
                    // 1. Dọn dẹp hiệu ứng DOTween rơi rớt đang chạy dở
                    rect.DOKill();
                    img.DOKill();

                    // 2. Trả lại góc xoay chuẩn (Layout Group sẽ tự lo vị trí X, Y)
                    rect.localRotation = Quaternion.identity;

                    // 3. Hiệu ứng nháy màu Xanh Lá báo hiệu hồi máu rồi từ từ về trắng
                    img.color = Color.green;
                    img.DOColor(Color.white, 0.5f);
                }
            }
        }
    }

    public bool isInvulnerable { get; private set; } = false;

    private PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();

        // ĐỒNG BỘ CHỈ SỐ MÁU TỪ KHO VĨNH CỬU KHI VỪA VÀO MAP
        if (PlayerDataManager.Instance != null)
        {
            currentHealth = PlayerDataManager.Instance.currentHealth;
            maxHealth = PlayerDataManager.Instance.maxHealth;
        }
        else
        {
            // Đề phòng chạy test Scene đơn lẻ chưa có PlayerDataManager ngoài Map
            maxHealth = 5;
            currentHealth = maxHealth;
        }

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

        // BÁO CÁO lại cho Kho lưu trữ để nó nhớ lượng máu mới
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.currentHealth = currentHealth;
        }

        // Hiệu ứng vỡ UI (Truyền vào currentHealth để lấy đúng vị trí thanh kiếm bị gãy)
        if (currentHealth >= 0 && currentHealth < swords.Count)
        {
            AnimateSwordBreaking(currentHealth);
        }

        if (currentHealth <= 0)
        {
            controller.stateMachine.ChangeState(controller.deadState);
        }
        else
        {
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

    public void InitHPUI()
    {
        // 1. Xóa sạch các thanh kiếm cũ (nếu có)
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

        // 3. Làm tối các thanh kiếm đã mất phòng trường hợp đổi Scene khi đang thấp máu
        for (int i = currentHealth; i < maxHealth; i++)
        {
            if (i < swords.Count)
            {
                Image img = swords[i].GetComponent<Image>();
                if (img != null) img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
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
            // Hủy các DOTween cũ đang chạy trên thanh kiếm này
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
    // ==========================================
    // HÀM HỒI ĐẦY MÁU KHI DỊCH CHUYỂN / LOAD GAME
    // ==========================================
    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;

        // Vòng lặp bật sáng lại toàn bộ UI Thanh Kiếm (Dùng .Count vì đây là List)
        for (int i = 0; i < swords.Count; i++)
        {
            if (swords[i] != null)
            {
                Image swordImg = swords[i].GetComponent<Image>();
                RectTransform rect = swords[i].GetComponent<RectTransform>();

                if (rect != null && swordImg != null)
                {
                    // 1. Hủy các hiệu ứng DOTween đang chạy dở (nếu có)
                    rect.DOKill();
                    swordImg.DOKill();

                    // 2. Khôi phục lại màu trắng sáng nguyên bản
                    swordImg.color = Color.white;

                    // 3. Khôi phục góc xoay thẳng đứng (đề phòng lúc nãy bị rung lắc)
                    rect.localRotation = Quaternion.identity;
                }
            }
        }
        Debug.Log(">>> Đã bơm đầy máu và reset UI thanh kiếm!");
    }
}