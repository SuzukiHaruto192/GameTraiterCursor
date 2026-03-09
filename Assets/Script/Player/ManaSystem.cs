using UnityEngine;
using UnityEngine.UI;

public class ManaSystem : MonoBehaviour
{
    [Header("UI Reference")]
    public Image manaFillImage;

    [Header("Mana Stats")]
    public float maxMana = 100f;
    public float currentMana = 0f;

    void Start()
    {
        UpdateManaUI();
    }
    public void AddMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        UpdateManaUI();
    }
    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            UpdateManaUI();
            return true;
        }
        else
        {
            Debug.Log("Không đủ Mana!");
            return false;
        }
    }
    private void UpdateManaUI()
    {
        if (manaFillImage != null)
        {
            manaFillImage.fillAmount = currentMana / maxMana;
        }
    }
}