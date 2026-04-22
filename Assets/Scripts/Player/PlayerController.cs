using UnityEngine;
using System.Collections; // Bắt buộc phải có dòng này để dùng IEnumerator

public class PlayerController : MonoBehaviour
{
    // ==========================================
    // --- 1. KHAI BÁO BIẾN TRẠNG THÁI & THÔNG SỐ ---
    // ==========================================

    [Header("--- THÔNG SỐ DI CHUYỂN ---")]
    public float speed = 10f;
    public float jumpForce = 12f;
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 3f;

    [Header("--- THÔNG SỐ DASH ---")]
    public float dashForce = 30f;
    public float dashTime = 0.2f;
    public ParticleSystem windEffect;
    public GameObject ghostPrefab;
    public float ghostDelay = 0.05f;

    [Header("--- KIỂM TRA MẶT ĐẤT & TƯỜNG ---")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    public Transform wallCheck;
    public Vector2 wallCheckSize = new Vector2(0.2f, 0.8f);
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;

    [Header("--- COYOTE TIME ---")]
    public float wallCoyoteTime = 0.15f;
    [HideInInspector] public float wallCoyoteTimer;
    [HideInInspector] public int lastWallDirection;

    [Header("--- CÀI ĐẶT TẤN CÔNG ---")]
    public int attackDamage = 100;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public HitEffectPlayer effectController;
    public float pogoBounceForce = 15f;

    [Header("HITBOX: CHẾM DƯỚI ĐẤT (Pos X,Y | Size X,Y)")]
    public Vector2 atk1Pos = new Vector2(1f, 0f); public Vector2 atk1Size = new Vector2(1.5f, 0.5f);
    public Vector2 atk2Pos = new Vector2(1f, 0f); public Vector2 atk2Size = new Vector2(1.5f, 0.5f);
    public Vector2 atk3Pos = new Vector2(1.2f, 0f); public Vector2 atk3Size = new Vector2(2f, 0.8f);

    [Header("HITBOX: CHẾM TRÊN KHÔNG")]
    public Vector2 airForwardPos = new Vector2(1f, 0f); public Vector2 airForwardSize = new Vector2(1.5f, 0.6f);
    public Vector2 airUpPos = new Vector2(0f, 1f); public Vector2 airUpSize = new Vector2(1.5f, 1f);
    public Vector2 airDownPos = new Vector2(0f, -1f); public Vector2 airDownSize = new Vector2(1.5f, 1f);

    [HideInInspector] public Vector2 currentHitboxSize;

    [Header("--- CỜ TRẠNG THÁI (Lưu trữ Logic) ---")]
    public bool isFacingRight = true;
    public bool canDash = true;
    public bool canAirAttack = true;
    public bool canDoubleJump = true;
    public bool isInvincible = false;
    public bool isDropping = false; // [MỚI] Cờ hiệu lọt xuống bệ 1 chiều

    // --- COMPONENTS CHÍNH ---
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public PlayerHealth playerHealth { get; private set; }
    public PlayerFX playerFX { get; private set; }

    // --- HỆ THỐNG STATE MACHINE ---
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerGroundedState groundedState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerHurtState hurtState { get; private set; }
    public PlayerDeadState deadState { get; private set; }

    public GroundAttack1State attack1State { get; private set; }
    public GroundAttack2State attack2State { get; private set; }
    public GroundAttack3State attack3State { get; private set; }
    public AirAttackUpState airAttackUpState { get; private set; }
    public AirAttackDownState airAttackDownState { get; private set; }
    public AirAttackForwardState airAttackForwardState { get; private set; }

    private Coroutine ghostCoroutine;

    // ==========================================
    // --- 2. CÁC HÀM CỦA UNITY ---
    // ==========================================

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        playerFX = GetComponent<PlayerFX>();

        stateMachine = new PlayerStateMachine();

        groundedState = new PlayerGroundedState(this, stateMachine, "");
        airState = new PlayerAirState(this, stateMachine, "");
        dashState = new PlayerDashState(this, stateMachine, "");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        hurtState = new PlayerHurtState(this, stateMachine, "");
        deadState = new PlayerDeadState(this, stateMachine, "");

        attack1State = new GroundAttack1State(this, stateMachine, "");
        attack2State = new GroundAttack2State(this, stateMachine, "");
        attack3State = new GroundAttack3State(this, stateMachine, "");
        airAttackUpState = new AirAttackUpState(this, stateMachine, "");
        airAttackDownState = new AirAttackDownState(this, stateMachine, "");
        airAttackForwardState = new AirAttackForwardState(this, stateMachine, "");
    }

    private void Start()
    {
        stateMachine.Initialize(groundedState);
    }

    private void Update()
    {
        stateMachine.CurrentState.LogicUpdate();
    }

    // ==========================================
    // --- 3. CÁC HÀM VẬT LÝ & KIỂM TRA MÔI TRƯỜNG ---
    // ==========================================

    public bool IsGrounded()
    {
        // Khi đang lọt xuống, báo cho cả Code và Animator biết là đã rời đất
        if (isDropping)
        {
            anim.SetBool("Grounded", false); // Ép Animator cập nhật ngay lập tức
            return false;
        }

        bool isGnd = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
        anim.SetBool("Grounded", isGnd);
        return isGnd;
    }

    public bool IsTouchingWall()
    {
        return Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer);
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
        anim.SetFloat("AirSpeedY", rb.linearVelocity.y);
    }

    private void FlipController(float xVelocity)
    {
        if (xVelocity > 0 && !isFacingRight) Flip();
        else if (xVelocity < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    // [MỚI] Hàm xử lý lọt qua bệ đỡ 1 chiều
    public IEnumerator DropThroughOneWayPlatform()
    {
        // Quét tìm cái bệ OneWayGround dưới chân
        Collider2D oneWayPlatform = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, LayerMask.GetMask("OneWayGround"));

        if (oneWayPlatform != null)
        {
            isDropping = true;
            Collider2D playerCollider = GetComponent<Collider2D>();

            // Tạm thời tắt va chạm vật lý
            Physics2D.IgnoreCollision(playerCollider, oneWayPlatform, true);

            // Chờ nhân vật rớt lọt qua (0.35s thường là đủ)
            yield return new WaitForSeconds(0.35f);

            // Bật lại va chạm bình thường
            Physics2D.IgnoreCollision(playerCollider, oneWayPlatform, false);
            isDropping = false;
        }
    }

    // ==========================================
    // --- 4. CÁC HÀM CHIẾN ĐẤU ---
    // ==========================================

    public void CheckAttackHitbox()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, currentHitboxSize, 0f, enemyLayers);

        bool hasHitSomething = false;
        bool hasPogoBounced = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            hasHitSomething = true;

            Vector2 exactHitPoint = enemy.ClosestPoint(attackPoint.position);
            if (effectController != null) effectController.PlayHitEffect(exactHitPoint);

            EnemyHealthBase enemyHealth = enemy.GetComponent<EnemyHealthBase>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, transform.position);
                // KÍCH HOẠT CẢM GIÁC ĐÁNH:
                if (playerFX != null) playerFX.TriggerHitStop();
                if (playerFX != null) playerFX.TriggerCameraShake();
            }

            // Xử lý riêng lực nảy Pogo
            if (stateMachine.CurrentState == airAttackDownState && !hasPogoBounced)
            {
                SetVelocity(rb.linearVelocity.x, pogoBounceForce);
                hasPogoBounced = true;
            }
        }

        if (hasHitSomething)
        {
            canDash = true;
            canAirAttack = true;
            canDoubleJump = true;
        }
    }

    public void FinishState()
    {
        stateMachine.CurrentState.AnimationFinishTrigger();
    }

    // ==========================================
    // --- 5. CÁC HÀM HIỆU ỨNG HÌNH ẢNH ---
    // ==========================================

    public void StartDashEffect()
    {
        if (windEffect != null) windEffect.Play();
        ghostCoroutine = StartCoroutine(CreateGhostRoutine());
    }

    public void StopDashEffect()
    {
        if (windEffect != null) windEffect.Stop();
        if (ghostCoroutine != null) StopCoroutine(ghostCoroutine);
    }

    private IEnumerator CreateGhostRoutine()
    {
        yield return null;
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
        while (true)
        {
            if (ghostPrefab != null)
            {
                GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
                GhostEffect ghostEffect = ghost.GetComponent<GhostEffect>();
                if (ghostEffect != null)
                    ghostEffect.SetGhost(playerSR.sprite, playerSR.flipX, transform.localScale);
            }
            yield return new WaitForSeconds(ghostDelay);
        }
    }

    // ==========================================
    // --- 6. HỖ TRỢ VẼ TRÊN EDITOR ---
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        if (wallCheck != null) Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Vector2 drawSize = currentHitboxSize != Vector2.zero ? currentHitboxSize : new Vector2(1f, 1f);
            Gizmos.DrawWireCube(attackPoint.position, drawSize);
            Gizmos.color = Color.white;
        }
    }
}