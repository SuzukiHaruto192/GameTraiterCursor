using UnityEngine;

public class CharmMenuController : MonoBehaviour
{
    [Header("Kéo tất cả các thành phần cần ẩn vào danh sách này")]
    public GameObject[] menuElements;

    private bool isMenuOpen = false;

    void Start()
    {
        SetMenuState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isMenuOpen = !isMenuOpen;
            SetMenuState(isMenuOpen);
        }
    }

    void SetMenuState(bool state)
    {
        if (menuElements == null) return;

        foreach (GameObject element in menuElements)
        {
            if (element != null)
            {
                element.SetActive(state);
            }
        }
    }
}