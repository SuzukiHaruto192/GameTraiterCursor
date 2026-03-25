using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 30f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float animationDelay = 0.3f;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float ghostDelay = 0.3f;
    [SerializeField] private ParticleSystem windEffect;
    [SerializeField] private float fallingForce = 1.5f;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private bool isGrounded = true;
    private bool isReversed = false;
    private bool isDashing = false;
    private bool hasDashedInAir = false;

    Rigidbody2D rb;
    Animator anim;

    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        anim.SetBool("isIdle", true);
        anim.SetBool("isWalk", false);
        // ❌ bỏ isAttack
    }

    void Update()
    {
        if (isDashing) return;

        if (playerHealth != null && playerHealth.isKnockedBack)
        {
            anim.SetBool("isWalk", false);
            return;
        }

        if (isGrounded)
        {
            hasDashedInAir = false;
        }

        float moveInput = Input.GetAxis("Horizontal");

        // xoay mặt
        if ((isReversed && moveInput > 0) || (!isReversed && moveInput < 0))
        {
            transform.Rotate(0, 180, 0);
            isReversed = !isReversed;
        }

        // di chuyển
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // animation Walk / Idle
        if (moveInput != 0)
        {
            anim.SetBool("isWalk", true);
            anim.SetBool("isIdle", false);
        }
        else
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isIdle", true);
        }

        // ✅ Jump → phím K
        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // tăng tốc rơi
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * fallingForce * Time.deltaTime;
        }

        // ❌ XÓA ATTACK Ở ĐÂY

        // ✅ Dash → phím L
        if (Input.GetKeyDown(KeyCode.L) && !hasDashedInAir)
        {
            StartCoroutine(Dash());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    IEnumerator Dash()
    {
        if (!isGrounded)
        {
            hasDashedInAir = true;
        }

        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        windEffect.Play();

        float dashDirection = isReversed ? -1 : 1;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0);

        Coroutine ghostCoroutine = StartCoroutine(CreateGhost());

        yield return new WaitForSeconds(dashTime);

        windEffect.Stop();
        StopCoroutine(ghostCoroutine);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    IEnumerator CreateGhost()
    {
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();

        while (true)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);

            GhostEffect ghostEffect = ghost.GetComponent<GhostEffect>();
            ghostEffect.SetGhost(playerSR.sprite, playerSR.flipX, playerSR.transform.localScale);

            yield return new WaitForSeconds(ghostDelay);
        }
    }

    private void FixedUpdate()
    {
        float curX = rb.position.x;

        if (curX <= minX)
        {
            rb.position = new Vector2(minX, rb.position.y);
            if (rb.linearVelocity.x < 0)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (curX >= maxX)
        {
            rb.position = new Vector2(maxX, rb.position.y);
            if (rb.linearVelocity.x > 0)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}