using UnityEngine;

public class UpgradePlayerStat : MonoBehaviour
{
    [SerializeField] private float amountToUpgrade;
    [SerializeField] private GameObject UpgradementUI;

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            { 
                UpgradementUI.SetActive(true);
            }
        }
    }
}
