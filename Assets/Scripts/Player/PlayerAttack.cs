using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDamage = 100;

    private PlayerMovement playerMove;
    public bool isInvincible = false;

    Rigidbody2D rb;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Tấn công
        if (Input.GetAxis("Vertical") < 0 && !playerMove.isGrounded && Input.GetKeyDown(KeyCode.J))     //Tấn công trên không --> Đè S và Ấn J
        {
            PerformAirSpecialSkill();
        }
        else if (Input.GetKeyDown(KeyCode.J))                                                           // Tấn công bình thường --> Ấn J
        {
            Attack();
        }

        // Đỡ đòn --> Ấn S hoặc mũi tên xuống
        if (Input.GetAxis("Vertical") < 0 && playerMove.isGrounded)
        {
            rb.linearVelocity = Vector3.zero;
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }

    }

    void Attack()
    {
        anim.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
        }
    }

    void PerformAirSpecialSkill()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}