using UnityEngine;
using System.Collections; // Cần thiết để sử dụng IEnumerator (đợi thời gian)

public class BossAttackController : MonoBehaviour
{
    [Header("Cài đặt Đạn/Laser Ngang")]
    // ---> ĐÃ CẬP NHẬT: Kéo Prefab Laser (có script SpriteBossLaser.cs) vào ô này <---
    public GameObject horizontalLaserPrefab;
    public Transform highFirePoint;          // Điểm bắn trên cao
    public Transform lowFirePoint;           // Điểm bắn dưới thấp

    [Header("Cài đặt Mưa Thiên Thạch (Dự phòng)")]
    public GameObject meteorPrefab;
    public Transform meteorSpawnArea;
    public float spawnWidth = 15f;
    public int meteorsPerWave = 5;
    public float timeBetweenMeteors = 0.2f;

    [Header("Thời gian chung")]
    public float fireRate = 3f;             // Cứ 3 giây Boss tung chiêu 1 lần
    private float nextFireTime;

    void Update()
    {
        // Kiểm tra xem đã đến giờ tung chiêu chưa
        if (Time.time >= nextFireTime)
        {
            // Boss tung đồng xu: 0 là bắn laser ngang, 1 là gọi thiên thạch
            // (Bạn có thể tăng tỉ lệ ra laser bằng cách chỉnh Random.Range(0, 5) và if attackType < 4 chẳng hạn)
            int attackType = Random.Range(0, 2);

            if (attackType == 0)
            {
                // Gọi Coroutine để xử lý việc bắn laser (cần đợi thời gian ngắm)
                StartCoroutine(FireHorizontalLaserRoutine());
            }
            else
            {
                // Gọi mưa thiên thạch (vẫn giữ nguyên như cũ)
                StartCoroutine(DropMeteors());
            }

            nextFireTime = Time.time + fireRate; // Hẹn giờ cho lần bắn tiếp theo
        }
    }

    // --- MỚI: Coroutine xử lý việc bắn laser ngang có ngắm ---
    IEnumerator FireHorizontalLaserRoutine()
    {
        Debug.Log(">>> Boss tung chiêu: LASER NGANG!");

        // 1. Random chọn điểm bắn cao hoặc thấp
        int randomChoice = Random.Range(0, 2);
        Transform selectedPoint = (randomChoice == 0) ? lowFirePoint : highFirePoint;

        if (horizontalLaserPrefab != null && selectedPoint != null)
        {
            // 2. TẠO TIA LASER:
            // Nó sẽ sinh ra ở FirePoint và TỰ ĐỘNG CHẠY HOẠT ẢNH NHẮM (Telegraph) 
            // nhờ script SpriteBossLaser.cs gắn trên nó.
            Instantiate(horizontalLaserPrefab, selectedPoint.position, selectedPoint.rotation);
        }

        // CHÚ Ý: Tại đây, Boss không cần phải đợi thời gian ngắm bắn nữa.
        // Script SpriteBossLaser.cs trên tia laser sẽ tự lo phần ngắm 1 giây,
        // sau đó tự phình to gây sát thương, rồi tự biến mất.

        yield return null;
    }

    // --- (Giữ nguyên) Coroutine gọi mưa thiên thạch ---
    IEnumerator DropMeteors()
    {
        Debug.Log(">>> Boss tung chiêu: MƯA THIÊN THẠCH!");
        if (meteorPrefab == null || meteorSpawnArea == null) yield break;

        for (int i = 0; i < meteorsPerWave; i++)
        {
            float randomX = meteorSpawnArea.position.x + Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
            Vector2 spawnPos = new Vector2(randomX, meteorSpawnArea.position.y);
            Quaternion rotation = Quaternion.Euler(0, 0, -90f);
            Instantiate(meteorPrefab, spawnPos, rotation);
            yield return new WaitForSeconds(timeBetweenMeteors);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ... (Phần vẽ đường kẻ đỏ trên trời vẫn giữ nguyên)
        if (meteorSpawnArea != null)
        {
            Gizmos.color = Color.red;
            Vector2 left = new Vector2(meteorSpawnArea.position.x - spawnWidth / 2f, meteorSpawnArea.position.y);
            Vector2 right = new Vector2(meteorSpawnArea.position.x + spawnWidth / 2f, meteorSpawnArea.position.y);
            Gizmos.DrawLine(left, right);
            Gizmos.DrawLine(left, left + Vector2.down);
            Gizmos.DrawLine(right, right + Vector2.down);
        }
    }
}