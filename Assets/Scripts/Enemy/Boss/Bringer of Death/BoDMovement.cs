using UnityEngine;

public class BoDMovement : MonoBehaviour
{
    [Header("Phạm vi Hang Boss")]
    public Transform leftBoundary;
    public Transform rightBoundary;

    [Header("Cảm biến Vực & Tường")]
    public float wallCheckDistance = 1.2f;
    public LayerMask groundLayer;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public void TeleportNearPlayer(float offset = 1.5f)
    {
        if (player == null) return;

        float dir = (Random.value > 0.5f) ? 1f : -1f;
        float targetX = player.position.x + (dir * offset);

        transform.position = CorrectTeleportPosition(targetX);
        LookAtPlayer();
    }

    public void TeleportRandom()
    {
        if (leftBoundary == null || rightBoundary == null) return;

        float randomX = Random.Range(leftBoundary.position.x, rightBoundary.position.x);
        transform.position = CorrectTeleportPosition(randomX);
        LookAtPlayer();
    }

    private Vector2 CorrectTeleportPosition(float targetX)
    {
        if (leftBoundary != null && rightBoundary != null)
        {
            targetX = Mathf.Clamp(targetX, leftBoundary.position.x, rightBoundary.position.x);
        }

        float startY = (player != null) ? player.position.y + 5f : transform.position.y + 5f;
        Vector2 rayOrigin = new Vector2(targetX, startY);
        RaycastHit2D groundHit = Physics2D.Raycast(rayOrigin, Vector2.down, 25f, groundLayer);

        float finalX = targetX;
        float finalY = (player != null) ? player.position.y : transform.position.y;

        if (groundHit.collider != null)
        {
            finalX = groundHit.point.x;
            finalY = groundHit.point.y;
        }
        else if (player != null)
        {
            finalX = player.position.x;
            finalY = player.position.y;
        }

        Vector2 wallCheckOrigin = new Vector2(finalX, finalY + 0.5f);
        RaycastHit2D wallRight = Physics2D.Raycast(wallCheckOrigin, Vector2.right, wallCheckDistance, groundLayer);
        if (wallRight.collider != null)
        {
            finalX = wallRight.point.x - wallCheckDistance;
        }

        RaycastHit2D wallLeft = Physics2D.Raycast(wallCheckOrigin, Vector2.left, wallCheckDistance, groundLayer);
        if (wallLeft.collider != null)
        {
            finalX = wallLeft.point.x + wallCheckDistance;
        }

        if (leftBoundary != null && rightBoundary != null)
        {
            finalX = Mathf.Clamp(finalX, leftBoundary.position.x, rightBoundary.position.x);
        }

        return new Vector2(finalX, finalY);
    }

    public void LookAtPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (leftBoundary != null && rightBoundary != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(leftBoundary.position.x, transform.position.y - 5f, 0), new Vector3(leftBoundary.position.x, transform.position.y + 5f, 0));
            Gizmos.DrawLine(new Vector3(rightBoundary.position.x, transform.position.y - 5f, 0), new Vector3(rightBoundary.position.x, transform.position.y + 5f, 0));
        }
    }
}