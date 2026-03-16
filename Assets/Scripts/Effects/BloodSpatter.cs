using UnityEngine;

public class BloodSpatter : MonoBehaviour
{
    // Hàm này sẽ được code máu của quái vật gọi để set hướng văng
    public void SetDirection(Vector2 direction)
    {
        // Tính toán góc xoay dựa trên hướng văng (từ Player đến Quái)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Xoay Particle System theo trục Z
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}