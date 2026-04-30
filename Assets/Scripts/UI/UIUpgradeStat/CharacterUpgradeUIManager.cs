using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UpgradementUI;
    [SerializeField] private Slider maxHealthBar;
    [SerializeField] private Slider attackBar;
    [SerializeField] private Slider jumpForceBar;
    [SerializeField] private TextMeshProUGUI availableCurrency;
    [SerializeField] private GameObject player;

    private PlayerHealth health;
    private PlayerController controller;

    private void Start()
    {
        health = player.GetComponent<PlayerHealth>();
        controller = player.GetComponent<PlayerController>();

        UpdateAvailableCurrency(health.crystals);
    }

    public void UpgradeMaxHealth()
    {
        if (health.crystals <= 0)
        {
            Debug.Log("KHONG DU CRYSTAL DE UPGRADE STAT");
            return;
        }

        if (maxHealthBar.value < maxHealthBar.maxValue)
        {
            maxHealthBar.value++;
            health.maxHealth++;
            health.crystals--;
            UpdateAvailableCurrency(health.crystals);
        }
    }

    public void UpgradeAttack()
    {
        if (health.crystals <= 0)
        {
            Debug.Log("KHONG DU CRYSTAL DE UPGRADE STAT");
            return;
        }

        if (attackBar.value < attackBar.maxValue) 
        {
            attackBar.value++;
            controller.attackDamage += 2;
            health.crystals--;
            UpdateAvailableCurrency(health.crystals);
        }
    }

    public void UpgradeJumpforce()
    {
        if (health.crystals <= 0)
        {
            Debug.Log("KHONG DU CRYSTAL DE UPGRADE STAT");
            return;
        }

        if (jumpForceBar.value < jumpForceBar.maxValue)
        {
            jumpForceBar.value++;
            controller.jumpForce += 2;
            health.crystals--;
            UpdateAvailableCurrency(health.crystals);
        }
    }

    public void Quit()
    {
        UpgradementUI.SetActive(false);
    }

    private void UpdateAvailableCurrency(int curCrystal)
    {
        availableCurrency.text = $"Available Currency: {curCrystal} crytals";
    }
}
