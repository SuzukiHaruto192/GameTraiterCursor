using UnityEngine;

public class BossArenaDoor : MonoBehaviour
{
    [Header("Tường vô hình chặn cửa")]
    public Collider2D invisibleWall;

    [Header("Máu của Boss")]
    public BoDHealth bossHealth;

    private bool isLocked = false;

    void Start()
    {
        if (invisibleWall != null) invisibleWall.enabled = false; // Mặc định tắt
    }

    // Khi Player bước qua cửa vào hang
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && bossHealth != null && !bossHealth.isDead && !isLocked)
        {
            isLocked = true;
            invisibleWall.enabled = true; // Bật tường chặn lại
            Debug.Log(">>> BẮT ĐẦU ĐÁNH BOSS! KHÓA CỬA! <<<");
        }
    }

    void Update()
    {
        // Khi boss chết, mở khóa cửa
        if (isLocked && (bossHealth == null || bossHealth.isDead))
        {
            invisibleWall.enabled = false;
            isLocked = false;
            Debug.Log(">>> BOSS CHẾT! MỞ CỬA! <<<");
        }
    }
}