using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float dashForce = 30f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float ghostDelay = 0.1f;
    [SerializeField] private GameObject ghostPrefab;

    private bool isGrounded = true;
    private bool isReversed = false;
    private bool isDashing = false;
    private float originalGravity;
    private bool hasDashedInAir = false;

    SpriteRenderer sr;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

    }
    void Update()
    {
        if (isDashing) return;
        if (isGrounded)
            hasDashedInAir = false;

        //Di chuyển
        float moveInput = Input.GetAxis("Horizontal");

        if ((isReversed && moveInput > 0) || (!isReversed && moveInput < 0))            //Xoay mặt khi di chuyển ngược
        {
            isReversed = !isReversed;
            sr.flipX = isReversed;
        }
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);


        //Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }


        //Lướt
        if (Input.GetKeyDown(KeyCode.C) && !hasDashedInAir)
        {
            StartCoroutine(Dash());
        }

    }

    void LateUpdate()                                                                   // Dùng LateUpdate để giới hạn vị trí, tránh di chuyển khỏi khung hình
    {
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
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
            hasDashedInAir = true;

        isDashing = true;
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        Coroutine ghostCoroutine = StartCoroutine(createGhost());

        float dashDirection = isReversed ? -1 : 1;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashTime);

        StopCoroutine(ghostCoroutine);


        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    IEnumerator createGhost() 
    {
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
        
        while (true) 
        {
            GameObject currentGhost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            GhostEffect effect = currentGhost.GetComponent<GhostEffect>();
            effect.SetGhost(playerSR.sprite, playerSR.flipX, transform.localScale);

            yield return new WaitForSeconds(ghostDelay);
        }
    }
}
