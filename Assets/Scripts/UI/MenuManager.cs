using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }

    public void ContinueGame()
    {
        GameManager.Instance.Continue();
    }
}