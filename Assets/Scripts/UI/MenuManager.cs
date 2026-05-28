using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject optionsPanel;

    [SerializeField] private GameObject menuCursor;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (mainMenuButtons != null) mainMenuButtons.SetActive(true);
        if (menuCursor != null) menuCursor.SetActive(true);

        float savedMusic = 0.75f;
        float savedSFX = 0.75f;

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    public void OpenOptions()
    {
        mainMenuButtons.SetActive(false);
        optionsPanel.SetActive(true);
        if (menuCursor != null) menuCursor.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuButtons.SetActive(true);
        if (menuCursor != null) menuCursor.SetActive(true);
    }

    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        audioMixer.SetFloat("MusicVol", dB);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        audioMixer.SetFloat("SFXVol", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

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