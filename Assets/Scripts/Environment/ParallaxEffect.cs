using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Cài đặt Camera")]
    [Tooltip("Kéo Main Camera vào đây. Nếu để trống, script sẽ tự tìm Main Camera.")]
    public Transform mainCamera;

    [Header("Cài đặt Tốc độ (Parallax Multiplier)")]
    [Tooltip("Hệ số di chuyển theo trục X và Y.\n< 0: Dành cho Foreground (Di chuyển ngược hướng camera, tạo cảm giác rất gần).\n0: Đứng yên như nền đất thông thường.\n> 0 và < 1: Dành cho Background (Di chuyển theo camera, tạo cảm giác ở xa).")]
    public Vector2 parallaxMultiplier;

    private Vector3 lastCameraPosition;

    void Start()
    {
        // Tự động tìm Camera chính nếu bạn chưa gán
        if (mainCamera == null)
        {
            mainCamera = Camera.main.transform;
        }

        // Lưu lại vị trí ban đầu của camera
        lastCameraPosition = mainCamera.position;
    }

    void LateUpdate()
    {
        // Tính toán khoảng cách camera đã di chuyển trong khung hình này
        Vector3 deltaMovement = mainCamera.position - lastCameraPosition;

        // Di chuyển lớp Foreground này dựa trên khoảng cách camera đã đi và hệ số nhân
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0);

        // Cập nhật lại vị trí camera cho khung hình tiếp theo
        lastCameraPosition = mainCamera.position;
    }
}