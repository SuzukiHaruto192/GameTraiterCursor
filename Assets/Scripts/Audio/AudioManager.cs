using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Áp dụng Singleton để gọi ở mọi nơi mà không cần Find
    public static AudioManager Instance;

    [Header("--- HỆ THỐNG LOA ---")]
    public AudioSource bgmSource; // Loa phát nhạc nền (Lặp lại)
    public AudioSource sfxSource; // Loa phát hiệu ứng (Kêu 1 lần)

    [Header("--- NHẠC NỀN ---")]
    public AudioClip backgroundMusic;

    void Awake()
    {
        // Setup Singleton chuẩn
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ loa không bị mất khi qua màn khác
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Bật nhạc nền ngay khi vào game
        if (backgroundMusic != null && bgmSource != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // --- HÀM NÀY ĐỂ CÁC SCRIPT KHÁC GỌI ---
    public void PlaySFX(AudioClip clipToPlay)
    {
        if (clipToPlay != null && sfxSource != null)
        {
            // PlayOneShot cho phép phát đè nhiều âm thanh cùng lúc (VD: 3 con quái chết cùng lúc)
            sfxSource.PlayOneShot(clipToPlay);
        }
    }
}