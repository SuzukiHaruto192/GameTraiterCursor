using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Bắt buộc phải có để dùng List (Danh sách)

public class BossAttackController : MonoBehaviour
{
    [Header("Cài đặt Đạn/Laser Ngang")]
    public GameObject horizontalLaserPrefab;
    public Transform highFirePoint;
    public Transform lowFirePoint;

    [Header("Cài đặt Mưa Thiên Thạch")]
    public GameObject meteorPrefab;
    public Transform meteorSpawnArea;
    public float spawnWidth = 15f;
    public int meteorsPerWave = 5;
    public float timeBetweenMeteors = 0.2f;

    [Header("Cài đặt Triệu hồi Quái (Slime)")]
    public GameObject slimePrefab;
    public Transform[] slimeSpawnPoints;
    public int slimesPerWave = 2;
    public float timeBetweenSpawns = 0.5f;

    [Header("Giới hạn Quái")]
    public int maxSlimesOnScreen = 4;        // Số lượng Slime tối đa được phép có trên sân
    [Range(0f, 1f)]
    [Tooltip("Mốc mở khóa (0.25 = Còn sống 25%, tức là đã giết 75%)")]
    public float unlockSpawnRatio = 0.25f;

    private List<GameObject> activeSlimes = new List<GameObject>(); // Cuốn sổ ghi danh Slime
    private bool isSpawningLocked = false;   // Cờ khóa kỹ năng đẻ quái
    // ----------------------------------------

    [Header("Thời gian chung")]
    public float fireRate = 3f;
    private float nextFireTime;

    private BossMovement movementScript;

    void Start()
    {
        movementScript = GetComponent<BossMovement>();
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            if (movementScript != null)
            {
                movementScript.HaltMovementForAttack(1f);
            }

            int attackType = Random.Range(0, 3);

            if (attackType == 0) StartCoroutine(FireHorizontalLaserRoutine());
            else if (attackType == 1) StartCoroutine(DropMeteors());
            else StartCoroutine(SpawnSlimesRoutine());

            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator FireHorizontalLaserRoutine()
    {
        Debug.Log(">>> Boss tung chiêu: LASER NGANG!");
        int randomChoice = Random.Range(0, 2);
        Transform selectedPoint = (randomChoice == 0) ? lowFirePoint : highFirePoint;

        if (horizontalLaserPrefab != null && selectedPoint != null)
        {
            Instantiate(horizontalLaserPrefab, selectedPoint.position, selectedPoint.rotation);
        }
        yield return null;
    }

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

    // --- CẬP NHẬT LOGIC KIỂM SOÁT SỐ LƯỢNG ---
    IEnumerator SpawnSlimesRoutine()
    {
        Debug.Log(">>> Boss chuẩn bị: TRIỆU HỒI SLIME!");
        if (slimePrefab == null || slimeSpawnPoints.Length == 0) yield break;

        // 1. DỌN SỔ: Tự động loại bỏ những con Slime đã bị Player chém chết (bị Destroy thành null)
        activeSlimes.RemoveAll(item => item == null);

        // 2. KIỂM TRA MỞ KHÓA: Nếu kỹ năng đang bị khóa do quá đông quái
        if (isSpawningLocked)
        {
            // Tính số lượng mốc. (Ví dụ Max=4, Ratio=0.25 => threshold = 1)
            int unlockThreshold = Mathf.FloorToInt(maxSlimesOnScreen * unlockSpawnRatio);

            if (activeSlimes.Count <= unlockThreshold)
            {
                isSpawningLocked = false;
                Debug.Log("Đã giết 75% Slime. MỞ KHÓA kỹ năng triệu hồi!");
            }
            else
            {
                Debug.Log($"Slime còn quá đông ({activeSlimes.Count}/{maxSlimesOnScreen}). HỦY tung chiêu đẻ.");
                // Dừng ngang tại đây, Boss không tung chiêu gì cả để lãng phí lượt, tạo cơ hội cho Player tấn công
                yield break;
            }
        }

        // 3. TIẾN HÀNH ĐẺ
        for (int i = 0; i < slimesPerWave; i++)
        {
            // Kiểm tra an toàn: Nếu đang đẻ giữa chừng mà chạm mốc Max thì ngừng ngay lập tức
            if (activeSlimes.Count >= maxSlimesOnScreen)
            {
                isSpawningLocked = true;
                break;
            }

            Transform randomSpawnPoint = slimeSpawnPoints[Random.Range(0, slimeSpawnPoints.Length)];
            GameObject newSlime = Instantiate(slimePrefab, randomSpawnPoint.position, Quaternion.identity);

            // Ghi tên con Slime vừa đẻ vào sổ
            activeSlimes.Add(newSlime);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        // 4. KIỂM TRA SAU KHI ĐẺ: Nếu chạm mốc thì khóa kỹ năng lại
        if (activeSlimes.Count >= maxSlimesOnScreen)
        {
            isSpawningLocked = true;
            Debug.Log("Đã đạt Max Slime. KHÓA kỹ năng triệu hồi!");
        }
    }

    private void OnDrawGizmosSelected()
    {
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