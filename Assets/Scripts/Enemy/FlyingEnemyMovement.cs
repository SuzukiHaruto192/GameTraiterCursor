using UnityEngine;

// Yêu cầu bắt buộc phải có Rigidbody2D và Animator trên cùng Object
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Cài đặt Di chuyển tuần tra (Ngang)")]
    // Tốc độ di chuyển ngang của quái
    [SerializeField] private float speed = 3f;
    // Khoảng cách quái sẽ bay về bên trái và bên phải từ vị trí gốc ban đầu
    [SerializeField] private float patrolRange = 4f;

    [Header("Cài đặt Dập dềnh (Lên xuống nhẹ)")]
    // Độ cao tối đa quái sẽ dập dềnh lên xuống (ví dụ 0.5f = dập dềnh trong khoảng 1m)
    [SerializeField] private float floatAmplitude = 0.5f;
    // Tốc độ dập dềnh (ví dụ 1.0f = 1 chu kỳ lên xuống mỗi giây)
    [SerializeField] private float floatFrequency = 1f;

    // Biến trạng thái
    private Vector2 startingPosition; // Vị trí gốc ban đầu của quái
    private int moveDirection = 1;    // 1 = đang đi sang phải, -1 = đang đi sang trái
    private float floatTimer;        // Thời gian nội bộ cho hàm hình Sin

    // Tham chiếu components
    private Rigidbody2D rb;
    private Animator anim;
    public bool canMove = true;

    void Start()
    {
        // Lấy các component khi bắt đầu
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Thiết lập vị trí gốc ban đầu
        startingPosition = transform.position;

        // --- CẤU HÌNH VẬT LÝ CHO QUÁI BAY ---
        // Để quái bay, ta cần Rigidbody2D là Kinematic hoặc Gravity Scale = 0
        rb.bodyType = RigidbodyType2D.Kinematic; // Dùng Kinematic để di chuyển mượt bằng code, né va chạm đẩy quái
        rb.useFullKinematicContacts = true; // Tùy chọn: giúp Kinematic xử lý va chạm với Ground/Tường tốt hơn

        // --- HỢP NHẤT ANIMATION (Ảnh 2) ---
        // Khi Start, quái luôn bắt đầu di chuyển, set isMove thành true
        anim.SetBool("isMove", true);
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        // Xác định ranh giới di chuyển dựa trên vị trí gốc
        float leftBound = startingPosition.x - patrolRange;
        float rightBound = startingPosition.x + patrolRange;

        // 1. TÍNH TOÁN DI CHUYỂN NGANG (Tuần tra)
        // Lấy vận tốc ngang hiện tại dựa trên tốc độ và hướng
        float horizontalVelocity = speed * moveDirection;

        // 2. TÍNH TOÁN DẬP DỀNH LÊN XUỐNG (Hàm hình Sin)
        // Cập nhật timer dập dềnh dựa trên tần số
        floatTimer += Time.fixedDeltaTime * floatFrequency;

        // Sử dụng Mathf.Sin để tạo chuyển động lên xuống mượt mà từ -1 đến 1, sau đó nhân với biên độ
        float currentOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        // Vị trí trục Y mục tiêu là vị trí gốc Y cộng với khoảng cách dập dềnh
        float targetY = startingPosition.y + currentOffset;

        // 3. THỰC HIỆN DI CHUYỂN VẬT LÝ MƯỢT MÀ
        // Ta tính toán vị trí trục X mục tiêu cho khung hình này
        float nextPositionX = transform.position.x + horizontalVelocity * Time.fixedDeltaTime;

        // Sử dụng rb.MovePosition để di chuyển Kinematic Object mượt mà và xử lý va chạm cơ bản
        // Nó sẽ move transform đến vị trí mục tiêu được tính toán gồm X tuần tra và Y dập dềnh
        rb.MovePosition(new Vector2(nextPositionX, targetY));

        // 4. KIỂM TRA RANH GIỚI VÀ QUAY ĐẦU (Quét ranh giới X)
        // Nếu quái đi quá ranh giới bên phải và đang đi sang phải, quay đầu
        if (transform.position.x >= rightBound && moveDirection == 1)
        {
            moveDirection = -1;
            FlipSprite();
        }
        // Nếu quái đi quá ranh giới bên trái và đang đi sang trái, quay đầu
        else if (transform.position.x <= leftBound && moveDirection == -1)
        {
            moveDirection = 1;
            FlipSprite();
        }
    }

    // Hàm quay đầu Sprites
    private void FlipSprite()
    {
        // Ta quay đầu bằng cách đảo chiều localScale.x
        Vector3 currentScale = transform.localScale;
        // Nhân localScale.x với -1 để đảo chiều
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    // Tùy chọn: Vẽ ranh giới di chuyển trong Scene để bạn dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        // Ta vẽ một đường Line ngang màu vàng trên Scene để thấy tầm di chuyển tuần tra
        Gizmos.color = Color.yellow;

        Vector2 center = Application.isPlaying ? startingPosition : (Vector2)transform.position;
        Vector2 leftLine = new Vector2(center.x - patrolRange, center.y);
        Vector2 rightLine = new Vector2(center.x + patrolRange, center.y);
        Gizmos.DrawLine(leftLine, rightLine);

        // Vẽ thêm một đường Line dọc màu xanh dương thể hiện độ cao dập dềnh
        Gizmos.color = Color.blue;
        Vector2 topLine = new Vector2(center.x, center.y + floatAmplitude);
        Vector2 bottomLine = new Vector2(center.x, center.y - floatAmplitude);
        Gizmos.DrawLine(topLine, bottomLine);
    }
}