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

    private bool isGrounded = true;
    private bool isReversed = false;
    private bool isDashing = false;
    private bool hasDashedInAir = false;

    Rigidbody2D rb;
    Animator anim;

    // liên kết với script máu
    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        anim.SetBool("isIdle", true);
        anim.SetBool("isWalk", false);
        anim.SetBool("isAttack", false);
    }

    void Update()
    {
        if (isDashing) return;

        // khóa điều khiển khi bị knockback
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

        // xoay mặt nhân vật
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

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // tăng tốc độ rơi
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }

        // attack
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Attack());
        }

        // dash
        if (Input.GetKeyDown(KeyCode.C) && !hasDashedInAir)
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

    IEnumerator Attack()
    {
        anim.SetBool("isAttack", true);
        anim.SetBool("isIdle", false);
        anim.SetBool("isWalk", false);

        yield return new WaitForSeconds(animationDelay);

        anim.SetBool("isAttack", false);
        anim.SetBool("isIdle", true);
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
}