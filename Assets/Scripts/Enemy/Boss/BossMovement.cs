using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float speed = 2f;
    public Transform leftPoint;      // Kéo điểm này ra sát tường trái
    public Transform rightPoint;     // Kéo điểm này ra sát tường phải

    [Header("Trạng thái")]
    public bool isAttacking = false;
    private bool movingRight = false;

    void Update()
    {
        // Đang tung chiêu thì đứng im
        if (isAttacking) return;
        if (leftPoint == null || rightPoint == null) return;

        // KIỂM TRA QUAY ĐẦU KHI ĐẾN ĐÍCH
        if (movingRight && transform.position.x >= rightPoint.position.x)
        {
            Flip();
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x)
        {
            Flip();
        }

        // LẦM LÌ BƯỚC TỚI (Đi xuyên qua mọi bục trên không)
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void HaltMovementForAttack(float pauseDuration)
    {
        StartCoroutine(PauseRoutine(pauseDuration));
    }

    IEnumerator PauseRoutine(float time)
    {
        isAttacking = true;
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }
}