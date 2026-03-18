using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Chỉ số Máu")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Hiển thị HP (kiếm)")]
    public GameObject[] swords;

    [Header("Cài đặt Phản hồi đòn đánh")]
    public float knockbackDuration = 0.2f;
    public float iFrameDuration = 1.2f;

    public bool isKnockedBack = false;
    private bool isInvulnerable = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateHPUI();
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        if (isInvulnerable) return;

        // 🔥 Mỗi hit = mất 1 kiếm
        currentHealth -= 1;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Trúng đòn! Còn lại: " + currentHealth);

        UpdateHPUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(KnockbackRoutine(knockbackForce));
            StartCoroutine(IFrameRoutine());
        }
    }

    void UpdateHPUI()
    {
        for (int i = 0; i < swords.Length; i++)
        {
            swords[i].SetActive(i < currentHealth);
        }
    }

    IEnumerator KnockbackRoutine(Vector2 force)
    {
        isKnockedBack = true;

        rb.linearVelocity = force;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        isKnockedBack = false;
    }

    IEnumerator IFrameRoutine()
    {
        isInvulnerable = true;

        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / 12);

            spriteRenderer.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(iFrameDuration / 12);
        }

        isInvulnerable = false;
    }

    void Die()
    {
        Debug.Log("Game Over!");
    }
}
