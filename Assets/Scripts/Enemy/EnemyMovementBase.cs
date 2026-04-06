using UnityEngine;
using System.Collections;

public class EnemyMovementBase : MonoBehaviour
{
    [Header("Trạng thái chung của Quái")]
    public bool canMove = true;

    [Header("Cài đặt Hiệu ứng Bị đánh (Hit Reaction)")]
    public float knockbackForce = 3f;      // Lực văng ra xa
    public float knockbackDuration = 0.1f; // Thời gian bị văng
    public float stunDuration = 0.5f;      // Tổng thời gian bị choáng (đứng im)

    // Hàm này sẽ được EnemyHealth gọi khi quái mất máu
    public void OnDamageTaken(Vector2 attackerPos)
    {
        StopAllCoroutines(); // Reset lại từ đầu nếu bị chém bồi liên tục
        StartCoroutine(HitReactionRoutine(attackerPos));
    }

    private IEnumerator HitReactionRoutine(Vector2 attackerPos)
    {
        // 1. Bắt đầu choáng -> Khóa di chuyển của mọi Lớp Con
        canMove = false;

        // 2. Thực hiện văng lùi (Knockback)
        float timer = 0f;
        Vector2 knockbackDir = ((Vector2)transform.position - attackerPos).normalized;

        while (timer < knockbackDuration)
        {
            transform.Translate(knockbackDir * knockbackForce * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        // 3. Đứng im chờ hết thời gian choáng
        // (Trừ đi khoảng thời gian đã dùng để bay lùi ở trên)
        if (stunDuration > knockbackDuration)
        {
            yield return new WaitForSeconds(stunDuration - knockbackDuration);
        }

        // 4. Giải trừ choáng -> Cho phép quái đi lại bình thường
        canMove = true;
    }
}