using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 30f;
    [SerializeField] private float dashTime = 0.2f;
    private bool isGrounded = true;
    private bool isReversed = false;
    private bool isDashing = false;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (isDashing) return;

        //Di chuyển
        float moveInput = Input.GetAxis("Horizontal");

        if ((isReversed && moveInput > 0) || (!isReversed && moveInput < 0))            //Xoay mặt khi di chuyển ngược
        {
            transform.Rotate(0, 180, 0);
            isReversed = !isReversed;
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        Vector2 curPos = transform.position;
        curPos.x = Mathf.Clamp(curPos.x, minX, maxX);
        transform.position = curPos;


        //Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        //Lướt
        if (Input.GetKeyDown(KeyCode.C))
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
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        float dashDirection = isReversed ? -1 : 1;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashTime);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;
        isDashing = false;
    }
}
