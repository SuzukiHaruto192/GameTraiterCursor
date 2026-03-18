using UnityEngine;

public class MenuCursorController : MonoBehaviour
{
    [Header("Cursor Settings")]
    public RectTransform cursor;
    public float speed = 15f;

    private Vector2 targetPosition;

    void Start()
    {
        if (cursor != null)
        {
            targetPosition = cursor.anchoredPosition;
        }
    }

    void Update()
    {
        if (cursor != null)
        {
            cursor.anchoredPosition = Vector2.Lerp(
                cursor.anchoredPosition,
                targetPosition,
                Time.deltaTime * speed
            );
        }
    }

    public void OnHoverButton(RectTransform buttonRect)
    {
        Debug.Log("Hover vào: " + buttonRect.name);
        targetPosition = new Vector2(
            cursor.anchoredPosition.x,
            buttonRect.anchoredPosition.y
        );
    }
}