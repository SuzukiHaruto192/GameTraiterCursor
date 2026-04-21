using UnityEngine;

public class CharacterUpgradeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UpgradementUI;
    public void Quit()
    {
        UpgradementUI.SetActive(false);
    }
}
