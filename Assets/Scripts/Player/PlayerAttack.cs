using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    public GameObject hitbox;
    public float attackCooldown = 0.5f;

    [Header("Optional")]
    public bool lockMovementWhileAttacking = true;

    private float lastAttackTime;
    private bool isAttacking = false;

    private Animator anim;
    private PlayerMovement movement; // để khóa di chuyển

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();

        hitbox.SetActive(false); // đảm bảo tắt lúc đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= lastAttackTime && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time + attackCooldown;

        anim.SetTrigger("Attack");

        // khóa di chuyển nếu bật
        if (lockMovementWhileAttacking && movement != null)
        {
            movement.enabled = false;
        }
    }

    // 👉 GỌI TRONG ANIMATION (frame bắt đầu chém)
    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }

    // 👉 GỌI TRONG ANIMATION (frame kết thúc chém)
    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }

    // 👉 GỌI Ở FRAME CUỐI ANIMATION
    public void EndAttack()
    {
        isAttacking = false;

        if (lockMovementWhileAttacking && movement != null)
        {
            movement.enabled = true;
        }
    }
}