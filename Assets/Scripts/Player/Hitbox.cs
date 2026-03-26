using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    public int damage = 10;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void OnEnable()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);

            // Truyền thêm vị trí của Hitbox vào làm hướng đánh
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage, transform.position);
        }
    }
}