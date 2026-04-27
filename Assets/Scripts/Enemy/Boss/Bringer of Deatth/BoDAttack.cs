using UnityEngine;

public class BoDAttack : MonoBehaviour
{
    [Header("Cấu hình Tấn công")]
    public float meleeRange = 4f;
    public float attackCooldown = 3f;
    private float nextAttackTime;

    [Header("Cài đặt Chém Cận Chiến")]
    public Collider2D meleeHitbox;

    private Transform player;
    private BoDMovement movementScript;
    private BoDAnimation animController;
    private BoDSpell spellSpawner;


    void Start()
    {
        movementScript = GetComponent<BoDMovement>();
        animController = GetComponent<BoDAnimation>();
        spellSpawner = GetComponent<BoDSpell>();

        // TÌM LỖI 1: Kiểm tra xem có tìm thấy Player không
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (meleeHitbox != null)
        {
            meleeHitbox.enabled = false;
        }
    }

    void Update()
    {
        if (player == null || Time.time < nextAttackTime) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < 100f)
        {
            ExecuteAIAction(distance);
        }
    }

    void ExecuteAIAction(float distance)
    {
        movementScript.SetLockState(true);

        if (distance <= meleeRange)
        {
            animController.TriggerAttack();
        }
        else
        {
            animController.TriggerCast();
        }

        nextAttackTime = Time.time + attackCooldown;
    }

    // ==========================================
    // CÁC HÀM GỌI TỪ ANIMATION EVENT
    // ==========================================
    public void EnableMeleeHitbox()
    {
        if (meleeHitbox != null) meleeHitbox.enabled = true;
    }

    public void DisableMeleeHitbox()
    {
        if (meleeHitbox != null) meleeHitbox.enabled = false;
    }

    public void ExecuteCastSequence()
    {

        if (spellSpawner != null && player != null)
        {
            float facingDirection = Mathf.Sign(transform.localScale.x) * -1f;
            spellSpawner.StartSpellSequence(player.transform, facingDirection);
            Debug.Log(">>> Đã gửi lệnh đẻ phép thành công!");
        }
    }

    public void UnlockBoss()
    {
        movementScript.SetLockState(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}