using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        // Nếu đang chạy trong Unity Editor thì dừng Playmode lại để test cho dễ
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Nếu là bản game đã xuất bản (Build) thì đóng hẳn ứng dụng game
        Application.Quit();
#endif
    }
}