using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Hiển thị HP (Kiếm UI)")]
    public GameObject[] swords;
    private Vector2[] initialPositions;
    private Quaternion[] initialRotations;

    public bool isInvulnerable { get; private set; } = false;

    private PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        currentHealth = maxHealth;

        initialPositions = new Vector2[swords.Length];
        initialRotations = new Quaternion[swords.Length];
        for (int i = 0; i < swords.Length; i++)
        {
            initialPositions[i] = swords[i].GetComponent<RectTransform>().anchoredPosition;
            initialRotations[i] = swords[i].GetComponent<RectTransform>().rotation;
        }

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

        // Hiệu ứng vỡ UI
        if (currentHealth >= 0 && currentHealth < swords.Length)
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

    void InitHPUI()
    {
        for (int i = 0; i < swords.Length; i++)
        {
            swords[i].SetActive(i < currentHealth);
            Image swordImg = swords[i].GetComponent<Image>();
            if (swordImg != null)
            {
                swordImg.color = new Color(1, 1, 1, 1);
                swords[i].GetComponent<RectTransform>().anchoredPosition = initialPositions[i];
                swords[i].GetComponent<RectTransform>().rotation = initialRotations[i];
            }
        }
    }

    void AnimateSwordBreaking(int index)
    {
        GameObject sword = swords[index];
        RectTransform rect = sword.GetComponent<RectTransform>();
        Image img = sword.GetComponent<Image>();

        if (rect != null && img != null)
        {
            rect.DOAnchorPosY(-50f, 0.4f).SetRelative().SetEase(Ease.InBack);
            rect.DORotate(new Vector3(0, 0, -30f), 0.4f).SetRelative();
            img.DOFade(0, 0.4f).OnComplete(() => sword.SetActive(false));
        }
    }
}