using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovementBase : MonoBehaviour
{
    [Header("Trạng thái chung của Quái")]
    public bool canMove = true;

    [Header("Cài đặt Hiệu ứng Bị đánh (Hit Reaction)")]
    public float knockbackForce = 7f;      // Chỉnh to lên tí vì dùng AddForce vật lý
    public float stunDuration = 0.5f;      // Tổng thời gian bị choáng

    protected Rigidbody2D rbBase;

    protected virtual void Awake()
    {
        rbBase = GetComponent<Rigidbody2D>();
    }

    // Thêm một biến kẹp giấy để quản lý riêng tác vụ "Bị đánh"
    private Coroutine hitCoroutine;

    public void OnDamageTaken(Vector2 attackerPos)
    {
        // 1. CHỈ dừng tờ giấy "Bị đánh" cũ (nếu có), tuyệt đối KHÔNG đụng đến tờ giấy "Tuần tra"
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        // 2. Phát tờ giấy "Bị đánh" mới và kẹp nó vào biến hitCoroutine
        hitCoroutine = StartCoroutine(HitReactionRoutine(attackerPos));
    }

    private IEnumerator HitReactionRoutine(Vector2 attackerPos)
    {
        canMove = false;

        // Xóa gia tốc cũ và tạo lực văng lùi (Vật lý chuẩn)
        if (rbBase != null)
        {
            rbBase.linearVelocity = Vector2.zero; // Dừng mọi di chuyển cũ

            // Tính hướng văng (đẩy ra xa khỏi Player)
            Vector2 knockbackDir = ((Vector2)transform.position - attackerPos).normalized;
            knockbackDir.y = 0.5f; // Ép nảy lên một chút cho có lực

            // Áp dụng lực (Quái sẽ tự bị cản lại nếu đụng tường)
            rbBase.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse);
        }

        // Đứng im chờ hết thời gian choáng
        yield return new WaitForSeconds(stunDuration);

        // Hết choáng -> Phanh quái lại để không bị trôi tuột đi như sân băng
        if (rbBase != null)
        {
            rbBase.linearVelocity = Vector2.zero;
        }

        canMove = true;
        OnStunEnd(); // Gọi hàm đồng bộ cho các lớp con
    }

    // Hàm ảo để các quái đặc biệt tự sửa dáng/tọa độ sau khi bị văng
    protected virtual void OnStunEnd() { }
}