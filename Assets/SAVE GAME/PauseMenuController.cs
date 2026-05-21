using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        _pauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        _pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void SaveAndQuit()
    {
        // Gọi lưu trực tiếp từ hệ thống mới gộp
        if (SaveSystem.Instance != null && PlayerDataManager.Instance != null)
        {
            SaveSystem.Instance.SaveGame(PlayerDataManager.Instance);
        }

        Debug.Log("Quit game");
        Application.Quit();
    }

    public void ReturnMainMenu()
    {
        if (SaveSystem.Instance != null && PlayerDataManager.Instance != null)
        {
            SaveSystem.Instance.SaveGame(PlayerDataManager.Instance);
        }

        Time.timeScale = 1; // Nhả thời gian đóng băng
        SceneManager.LoadScene("MenuScene");
    }
}