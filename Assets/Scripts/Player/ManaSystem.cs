using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMana : MonoBehaviour
{
    [Header("Chỉ số Mana")]
    public float maxMana = 100f;
    public float currentMana = 0f;

    [Header("Giao diện UI")]
    public Image manaFillImage;

    [Header("Cài đặt Lượng Mana Nhận Được")]
    public float manaGainedFromAttacking = 15f; // Đánh trúng quái được bao nhiêu
    public float manaGainedFromTakingDamage = 25f; // Bị đánh trúng được bao nhiêu

    void Start()
    {
        UpdateManaUI(0f);
    }

    public void GainManaFromAttack()
    {
        AddMana(manaGainedFromAttacking);
    }

    public void GainManaFromDamage()
    {
        AddMana(manaGainedFromTakingDamage);
    }

    private void AddMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana);

        UpdateManaUI(0.3f);
    }

    private void UpdateManaUI(float duration)
    {
        if (manaFillImage == null) return;

        float targetFill = currentMana / maxMana;

        manaFillImage.DOFillAmount(targetFill, duration).SetEase(Ease.OutCubic);
        manaFillImage.rectTransform.DORewind();
        manaFillImage.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
    }
}