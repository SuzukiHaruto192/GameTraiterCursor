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

    void Start()
    {
        movementScript = GetComponent<BoDMovement>();
        animController = GetComponent<BoDAnimation>();
        spellSpawner = GetComponent<BoDSpell>();
        healthScript = GetComponent<BoDHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (meleeHitbox != null) meleeHitbox.enabled = false;
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
            animController.TriggerAttack();
            yield return new WaitForSeconds(1f);
            nextStationaryTime = Time.time + stationaryAttackCD;
        }
        else if (actionType == "TeleportAttack")
        {
            animController.TriggerTeleport();
            yield return new WaitForSeconds(0.4f);
            movementScript.TeleportNearPlayer(1.5f);
            animController.TriggerAttack();
            yield return new WaitForSeconds(1f);
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
                animController.TriggerAttack();
                nextTeleportAttackTime = Time.time + teleportAttackCD;
                yield return new WaitForSeconds(1f);
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

    // ==========================================
    // HÀM MỚI: BỊ NGẮT CHIÊU KHI DÍNH ĐÒN
    // ==========================================
    public void InterruptAction()
    {
        if (isActing && !isCounterAttacking)
        {
            StopAllCoroutines();
            isActing = false;
            // Tắt hitbox lỡ kiếm đang vung xuống
            if (meleeHitbox != null) meleeHitbox.enabled = false;
        }
    }

    // ==========================================
    // KỸ NĂNG ĐẶC BIỆT: PHẢN ĐÒN NGAY LẬP TỨC
    // ==========================================
    public void TriggerCounterAttack()
    {
        StopAllCoroutines();
        StartCoroutine(CounterAttackRoutine());
    }

    IEnumerator CounterAttackRoutine()
    {
        isActing = true;
        isCounterAttacking = true; // BẬT GIÁP BÁ THỂ

        movementScript.LookAtPlayer();
        animController.TriggerAttack();

        yield return new WaitForSeconds(1f);

        nextBrainTickTime = Time.time + 0.2f;
        isActing = false;
        isCounterAttacking = false; // TẮT GIÁP BÁ THỂ
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