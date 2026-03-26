using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;

    public Animator animator;
    private bool isDead = false;

    public Rigidbody2D rb;

    private bool isHurt = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Debug.Log("Quái bị chém! Sát thương nhận: " + damage);

        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetBool("isDead", true);

        // Dành cho con bay
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(0, 2f);
        // GetComponent<Collider2D>().enabled = false;

        this.enabled = false;
    }
}