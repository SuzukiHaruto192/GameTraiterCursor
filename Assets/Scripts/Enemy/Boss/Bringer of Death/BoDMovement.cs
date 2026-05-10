using UnityEngine;

public class BoDMovement : MonoBehaviour
{
    [Header("Cấu hình Di chuyển")]
    public float speed = 2f;
    public float stopDistance = 3f; // Khoảng cách Boss sẽ dừng lại để bắt đầu đánh

    private Transform player;
    public bool isAttacking = false; // Biến này sẽ khóa di chuyển và xoay hướng
    private BoDAnimation animController;

    void Start()
    {
        animController = GetComponent<BoDAnimation>();
        // Tự động tìm người chơi theo Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isAttacking)
        {
            if (animController != null) animController.SetWalking(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // 1. Xử lý xoay hướng (Chỉ xoay khi không đang đánh)
        LookAtPlayer();

        // 2. Di chuyển về phía người chơi nếu còn ở xa
        if (distance > stopDistance)
        {
            Vector2 target = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (animController != null) animController.SetWalking(true);
        }
        else
        {
            if (animController != null) animController.SetWalking(false);
        }
    }

    void LookAtPlayer()
    {
        // Nếu Player bên phải Boss và Boss đang nhìn trái, hoặc ngược lại
        if (player.position.x > transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && transform.localScale.x < 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    // Hàm này được gọi bởi BoDAttack để khựng lại khi tung chiêu
    public void SetLockState(bool locked)
    {
        isAttacking = locked;
    }
}