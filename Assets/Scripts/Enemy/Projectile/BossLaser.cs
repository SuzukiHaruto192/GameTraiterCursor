using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class BossLaser : MonoBehaviour
{
    [Header("Cài đặt Thời gian (Giây)")]
    public float telegraphDuration = 1.0f; // 1. Hiện tia đỏ mờ báo trước
    public float expandDuration = 0.15f;   // 2. Thời gian chớp sáng và phình to (Rất nhanh)
    public float firingDuration = 0.5f;    // 3. Thời gian duy trì tia laser khổng lồ
    public float fadeDuration = 0.3f;      // 4. Thời gian mờ và tắt đi

    [Header("Kích thước (Theo trục Y)")]
    public float thinScale = 0.2f;         // Độ mỏng lúc ngắm bắn
    public float thickScale = 1.5f;        // Độ bự lúc bắn thật

    [Header("Màu sắc")]
    public Color telegraphColor = new Color(1f, 0f, 0f, 0.4f); // Đỏ mờ (Alpha = 0.4)
    public Color flashColor = Color.white;                     // Chớp lóe màu trắng
    public Color firingColor = new Color(1f, 0.2f, 0.2f, 1f);  // Đỏ rực rỡ (Alpha = 1)

    [Header("Sát thương")]
    public int damage = 10;

    private SpriteRenderer sr;
    private BoxCollider2D col;
    private Vector3 originalScale;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();

        // Lưu lại chiều dài gốc của hình ảnh (Scale X)
        originalScale = transform.localScale;

        // Mặc định tắt va chạm khi mới sinh ra
        col.isTrigger = true;
        col.enabled = false;
    }

    void Start()
    {
        // Khởi động chuỗi hoạt ảnh
        StartCoroutine(LaserAnimationSequence());
    }

    IEnumerator LaserAnimationSequence()
    {
        // ================= 1. HIỆN TIA ĐỎ (TELEGRAPH) =================
        sr.color = telegraphColor;

        // Ép dẹp hình ảnh lại theo trục Y
        transform.localScale = new Vector3(originalScale.x, thinScale, originalScale.z);

        yield return new WaitForSeconds(telegraphDuration);


        // ================= 2. CHỚP SÁNG & TO DẦN (EXPAND) =================
        col.enabled = true; // BẬT SÁT THƯƠNG NGAY TẠI ĐÂY

        float timer = 0;
        while (timer < expandDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / expandDuration; // Chạy từ 0 đến 1

            // Phình to ra từ từ
            float currentY = Mathf.Lerp(thinScale, thickScale, progress);
            transform.localScale = new Vector3(originalScale.x, currentY, originalScale.z);

            // Chớp từ Trắng sang Đỏ rực
            sr.color = Color.Lerp(flashColor, firingColor, progress);

            yield return null; // Đợi sang frame tiếp theo
        }

        // Đảm bảo thông số đạt chuẩn 100% sau khi kết thúc vòng lặp
        transform.localScale = new Vector3(originalScale.x, thickScale, originalScale.z);
        sr.color = firingColor;


        // ================= 3. DUY TRÌ BẮN =================
        yield return new WaitForSeconds(firingDuration);


        // ================= 4. TẮT ĐI (FADE) =================
        col.enabled = false; // TẮT SÁT THƯƠNG
        timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // Làm mờ đi (Alpha giảm từ 1 về 0)
            Color c = firingColor;
            c.a = Mathf.Lerp(1f, 0f, progress);
            sr.color = c;

            // (Tùy chọn) Ép dẹp tia laser lại cho giống anime tắt năng lượng
            float currentY = Mathf.Lerp(thickScale, 0f, progress);
            transform.localScale = new Vector3(originalScale.x, currentY, originalScale.z);

            yield return null;
        }

        // Xóa tia laser
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Áp dụng logic truyền vị trí gây giật lùi tương tự như Hitbox.cs
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage, transform.position);
            Debug.Log("Laser trúng đích!");
        }
    }
}