using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // Dùng cho TextMeshPro

public class ShowPanelOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Thông tin Charm (Điền riêng cho mỗi nút)")]
    [TextArea(3, 5)]
    public string charmDescription = "Mô tả công dụng của charm này...";
    [TextArea(3, 5)]
    public string charmStory = "Câu chuyện của charm này...";

    [Header("Text")]
    public TMP_Text displayDescription;
    public TMP_Text displayStory;

    [Header("Panel")]
    public GameObject hoverPanel;
    public GameObject selectedPanel;

    public float offsetY = 0f;

    [Header("Hover Settings (Cho Khung Mờ)")]
    [Range(0f, 1f)] public float hoverAlpha = 0.4f;
    public Color hoverColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("Active Settings (Cho Khung Sáng)")]
    public Color activeColor = Color.white;

    private static ShowPanelOnHover activeIcon = null;

    private CanvasGroup hoverCanvasGroup;
    private Image hoverImage;
    private CanvasGroup selectedCanvasGroup;
    private Image selectedImage;

    void Start()
    {
        if (hoverPanel != null)
        {
            hoverCanvasGroup = GetOrAddCanvasGroup(hoverPanel);
            hoverImage = hoverPanel.GetComponent<Image>();
            hoverCanvasGroup.alpha = hoverAlpha;
            if (hoverImage != null) hoverImage.color = hoverColor;
            hoverPanel.SetActive(false);
        }
        if (selectedPanel != null)
        {
            selectedCanvasGroup = GetOrAddCanvasGroup(selectedPanel);
            selectedImage = selectedPanel.GetComponent<Image>();
            selectedCanvasGroup.alpha = 1f;
            if (selectedImage != null) selectedImage.color = activeColor;
            selectedPanel.SetActive(false);
        }
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Rê chuột vào chỉ di chuyển khung mờ, KHÔNG cập nhật Text
        if (activeIcon != this)
        {
            hoverPanel.SetActive(true);
            UpdatePosition(hoverPanel, this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activeIcon == this)
        {
            activeIcon = null;
            selectedPanel.SetActive(false);

            hoverPanel.SetActive(true);
            UpdatePosition(hoverPanel, this);
            UpdateGlobalText("", "");
        }
        else
        {
            activeIcon = this;
            hoverPanel.SetActive(false);

            selectedPanel.SetActive(true);
            UpdatePosition(selectedPanel, this);
            UpdateGlobalText(charmDescription, charmStory);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPanel.SetActive(false);
    }

    private void UpdatePosition(GameObject pnl, ShowPanelOnHover targetIcon)
    {
        if (pnl == null) return;
        RectTransform pnlRect = pnl.GetComponent<RectTransform>();
        RectTransform targetRect = targetIcon.GetComponent<RectTransform>();
        pnlRect.anchoredPosition = new Vector2(-625f, targetRect.anchoredPosition.y + targetIcon.offsetY);
    }

    private void UpdateGlobalText(string desc, string story)
    {
        if (displayDescription != null) displayDescription.text = desc;
        if (displayStory != null) displayStory.text = story;
    }
}