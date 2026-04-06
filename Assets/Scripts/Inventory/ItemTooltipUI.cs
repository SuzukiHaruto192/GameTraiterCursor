using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance;

    [Header("UI References")]
    public GameObject tooltipObj;
    public Image itemIcon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    private RectTransform transform;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        transform = tooltipObj.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (tooltipObj.activeSelf)
        {
            Vector2 position = Input.mousePosition;
            position.x += 114f;
            position.y -= 78f;

            transform.position = position;
        }    
    }

    public void ShowTooltip(InventoryItem item)
    {
        if (item == null || item.itemData == null) return;

        titleText.text = item.itemData.name;
        descriptionText.text = item.itemData.itemDescription;
        itemIcon.sprite = item.itemData.icon;

        if (tooltipObj != null)
            tooltipObj.SetActive(true);
    }

    public void HideTooltip()
    { 
        tooltipObj.SetActive(false);
    }
}
