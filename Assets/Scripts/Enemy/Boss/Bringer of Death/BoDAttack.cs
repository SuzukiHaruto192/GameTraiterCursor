using UnityEngine;
using System.Collections;

public class BoDAttack : MonoBehaviour
{
    [Header("Cài đặt Phạm Vi (AI)")]
    public float meleeRange = 1.5f;
    public float shortRange = 5f;
    public float longRange = 10f;

    [Header("Cài đặt Hồi Chiêu (Cooldown)")]
    public float stationaryAttackCD = 4f;
    public float teleportAttackCD = 5f;
    public float spellCD = 2f;

    private float nextStationaryTime;
    private float nextTeleportAttackTime;
    private float nextSpellTime;
    private float nextBrainTickTime;

    [Header("Hitbox Cận Chiến")]
    public Collider2D meleeHitbox;

    private Transform player;
    private BoDMovement movementScript;
    private BoDAnimation animController;
    private BoDSpell spellSpawner;
    private BoDHealth healthScript;

    public bool isActing = false;
    public bool isCounterAttacking = false; // CHỈ BẬT KHI ĐANG PHẢN ĐÒN (GIÁP BÁ THỂ)

    // BIẾN MỚI: Cờ khóa quay người khi đang vung kiếm
    public bool isMeleeAttacking = false;

    void Start()
    {
        movementScript = GetComponent<BoDMovement>();
        animController = GetComponent<BoDAnimation>();
        spellSpawner = GetComponent<BoDSpell>();
        healthScript = GetComponent<BoDHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (meleeHitbox != null) meleeHitbox.enabled = false;
        // ==========================================
        // FIX LỖI SPAM CHIÊU LÚC MỚI VÀO GAME
        // ==========================================

        float wakeUpDelay = 2f; // Thời gian Boss đứng yên "nhìn" bạn lúc mới vào (2 giây)

        // Bắt bộ não tạm dừng hoạt động trong 2 giây đầu
        nextBrainTickTime = Time.time + wakeUpDelay;

        // Khởi tạo thời gian hồi chiêu ban đầu (Đánh so le ra để Boss không bấm 3 nút cùng 1 lúc)
        nextStationaryTime = Time.time + wakeUpDelay;
        nextTeleportAttackTime = Time.time + wakeUpDelay + 1.5f; // Tốc biến sẽ dùng sau 1.5 giây
        nextSpellTime = Time.time + wakeUpDelay + 3f;
    }

    void Update()
    {
        if (player == null || isActing || healthScript.isDead) return;

        if (Time.time < nextBrainTickTime) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= longRange)
        {
            DecideAction(distance);
        }
    }

    void DecideAction(float distance)
    {
        // Chốt hướng nhìn về phía Player trước khi quyết định ra đòn
        movementScript.LookAtPlayer();

        if (distance <= meleeRange)
        {
            if (Time.time >= nextStationaryTime)
                StartCoroutine(ActionRoutine("StationaryAttack"));
            return;
        }

        bool canTeleport = Time.time >= nextTeleportAttackTime;
        bool canSpell = Time.time >= nextSpellTime;

        if (!canTeleport && !canSpell) return;

        float roll = Random.Range(0f, 1f);

        if (distance <= shortRange)
        {
            if (roll <= 0.66f)
            {
                if (canTeleport) StartCoroutine(ActionRoutine("TeleportAttack"));
                else if (canSpell) StartCoroutine(ActionRoutine("Spell"));
            }
            else
            {
                if (canSpell) StartCoroutine(ActionRoutine("Spell"));
                else if (canTeleport) StartCoroutine(ActionRoutine("TeleportAttack"));
            }
        }
        else if (distance <= longRange)
        {
            if (roll <= 0.66f)
            {
                if (canSpell) StartCoroutine(ActionRoutine("Spell"));
                else if (canTeleport) StartCoroutine(ActionRoutine("RandomTeleport"));
            }
            else
            {
                if (canTeleport) StartCoroutine(ActionRoutine("RandomTeleport"));
                else if (canSpell) StartCoroutine(ActionRoutine("Spell"));
            }
        }
    }

    IEnumerator ActionRoutine(string actionType)
    {
        isActing = true;

        if (actionType == "StationaryAttack")
        {
            isMeleeAttacking = true; // KHÓA QUAY NGƯỜI
            animController.TriggerAttack();
            yield return new WaitForSeconds(1f);
            isMeleeAttacking = false; // MỞ KHÓA

            nextStationaryTime = Time.time + stationaryAttackCD;
        }
        else if (actionType == "TeleportAttack")
        {
            animController.TriggerTeleport();
            yield return new WaitForSeconds(0.4f);

            movementScript.TeleportNearPlayer(1.5f);
            movementScript.LookAtPlayer(); // Cập nhật hướng lần cuối sau khi tốc biến tới gần

            isMeleeAttacking = true; // KHÓA QUAY NGƯỜI
            animController.TriggerAttack();
            yield return new WaitForSeconds(1f);
            isMeleeAttacking = false; // MỞ KHÓA

            nextTeleportAttackTime = Time.time + teleportAttackCD;
        }
        else if (actionType == "Spell")
        {
            animController.TriggerCast();
            yield return new WaitForSeconds(1.5f);
            nextSpellTime = Time.time + spellCD;
        }
        else if (actionType == "RandomTeleport")
        {
            animController.TriggerTeleport();
            yield return new WaitForSeconds(0.4f);
            movementScript.TeleportRandom();

            float newDist = Vector2.Distance(transform.position, player.position);
            if (newDist <= meleeRange)
            {
                movementScript.LookAtPlayer(); // Cập nhật hướng lần cuối
                isMeleeAttacking = true; // KHÓA QUAY NGƯỜI
                animController.TriggerAttack();

                nextTeleportAttackTime = Time.time + teleportAttackCD;
                yield return new WaitForSeconds(1f);

                isMeleeAttacking = false; // MỞ KHÓA
            }
            else
            {
                animController.TriggerCast();
                nextSpellTime = Time.time + spellCD;
                yield return new WaitForSeconds(1.5f);
            }
        }

        nextBrainTickTime = Time.time + 0.5f;
        isActing = false;
    }

    public void InterruptAction()
    {
        if (isActing && !isCounterAttacking)
        {
            StopAllCoroutines();
            isActing = false;
            isMeleeAttacking = false; // Reset luôn cờ lỡ như bị ngắt giữa chừng

            if (meleeHitbox != null) meleeHitbox.enabled = false;
        }
    }

    public void TriggerCounterAttack()
    {
        StopAllCoroutines();
        StartCoroutine(CounterAttackRoutine());
    }

    IEnumerator CounterAttackRoutine()
    {
        isActing = true;
        isCounterAttacking = true;

        movementScript.LookAtPlayer(); // Chốt hướng nhìn trước khi chém

        isMeleeAttacking = true; // KHÓA QUAY NGƯỜI
        animController.TriggerAttack();

        yield return new WaitForSeconds(1f);

        nextBrainTickTime = Time.time + 0.2f;
        isActing = false;
        isCounterAttacking = false;
        isMeleeAttacking = false; // MỞ KHÓA
    }

    public void EnableMeleeHitbox() { if (meleeHitbox != null) meleeHitbox.enabled = true; }
    public void DisableMeleeHitbox() { if (meleeHitbox != null) meleeHitbox.enabled = false; }

    public void ExecuteCastSequence()
    {
        if (spellSpawner != null && player != null)
        {
            float facingDirection = Mathf.Sign(transform.localScale.x) * -1f;
            spellSpawner.StartSpellSequence(player.transform, facingDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shortRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, longRange);
    }
}