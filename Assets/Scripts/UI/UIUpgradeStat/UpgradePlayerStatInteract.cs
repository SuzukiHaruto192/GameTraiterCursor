using UnityEngine;

public class UpgradePlayerStat : MonoBehaviour
{
    [SerializeField] private GameObject UpgradementUI;
    [SerializeField] private GameObject interactionHintUI;
    private bool isPlayerInRange = false;

    private void Start()
    {
        interactionHintUI.transform.position = new Vector3(transform.position.x, transform.position.y + 4.5f, -1f);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            UpgradementUI.SetActive(true);
            interactionHintUI.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionHintUI.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionHintUI.SetActive(false);
        }
    }
}
