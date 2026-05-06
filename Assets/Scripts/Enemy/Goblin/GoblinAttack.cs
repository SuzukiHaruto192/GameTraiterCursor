using UnityEngine;
using System.Collections;

public class GoblinAttack : MonoBehaviour
{
    [Header("--- CÀI ĐẶT TẤN CÔNG ---")]
    public float attackRange = 1.5f;      // Phải bằng với attackRange bên GoblinMovement
    public float attackCooldown = 2f;     // Cứ 2 giây chém 1 lần

    [Header("--- SÁT THƯƠNG ---")]
    public int damage = 10;
    public float knockbackPower = 15f;    // Lực văng truyền cho Player

    [Header("--- VÙNG CHÉM ---")]
    public Transform attackPoint;         // Tạo 1 object rỗng đặt trước mặt con Goblin
    public float attackRadius = 0.8f;
    public LayerMask playerLayer;

    private Animator anim;
    private GoblinMovement movementScript;
    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<GoblinMovement>();

        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
    }

    void Update()
    {
        // Nếu Player chết, đang mải chém, hoặc đang bị Stun văng lùi thì không chém
        if (player == null || isAttacking || !movementScript.canMove) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // LỌT VÀO TẦM CHÉM!
        if (distToPlayer <= attackRange)
        {
            movementScript.FacePlayer(); // Quay mặt nhìn thẳng mặt Player
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. KHÓA DI CHUYỂN VÀ ĐỨNG LẠI
        movementScript.canMove = false;
        movementScript.StopMovingAnimation();

        // 2. KÍCH HOẠT ANIMATION
        anim.SetTrigger("Attack");

        // 3. THỜI GIAN GỒNG (Sửa số 0.4f này cho khớp với lúc cái kiếm chém xuống trong Animation)
        yield return new WaitForSeconds(0.4f);

        // 4. QUÉT SÁT THƯƠNG
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (hit != null)
        {
            Vector2 knockbackDir = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0.2f).normalized;
            hit.GetComponent<PlayerHealth>().TakeDamage(damage, knockbackDir * knockbackPower);
        }

        // 5. ĐỢI CHÉM XONG
        yield return new WaitForSeconds(0.6f); // Tổng thời gian gồng + chém xong là 1 giây

        // 6. TRẢ LẠI QUYỀN DI CHUYỂN
        movementScript.canMove = true;

        // 7. THỜI GIAN NGHỈ COOLDOWN (Đã tốn 1s chém, nghỉ thêm 1s nữa là tròn 2 giây)
        yield return new WaitForSeconds(attackCooldown - 1f);

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = new Color(1, 0.5f, 0); // Màu cam
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}