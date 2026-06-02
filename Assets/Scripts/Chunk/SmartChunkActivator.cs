using UnityEngine;
using System.Collections;

public class SmartChunkActivator : MonoBehaviour
{
    [Header("--- CÀI ĐẶT BẬT/TẮT CHUNK ---")]
    [Tooltip("Khoảng cách để bật/tắt nhóm quái vật này")]
    public float activationDistance = 30f;

    [Header("--- CÀI ĐẶT SPAWN QUÁI TỰ ĐỘNG ---")]
    [Tooltip("Kéo thả Prefab của các loại quái vào đây")]
    public GameObject[] enemyPrefabs;
    public float spawnInterval = 5f;  // Thời gian chờ giữa 2 lần đẻ quái
    public int spawnAmount = 2;       // Số lượng quái đẻ ra trong 1 lần
    public int maxEnemies = 10;       // Số lượng quái TỐI ĐA được phép tồn tại trong Chunk

    [Header("--- VỊ TRÍ SPAWN (Tương đối so với tâm Chunk) ---")]
    public float minSpawnX = -5f;
    public float maxSpawnX = 5f;
    public float fixedSpawnY = 0f;

    private Transform player;
    private bool isChunkActive = true;
    private GameObject contentHolder;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player;
        }
        else
        {
            Debug.LogWarning("SmartChunkActivator: Không tìm thấy Player trong GameManager!");
        }

        CreateContentHolder();

        // Chạy song song 2 luồng: 1 luồng check khoảng cách, 1 luồng đẻ quái
        StartCoroutine(CheckDistanceRoutine());
        StartCoroutine(SpawnEnemyRoutine());
    }

    private void CreateContentHolder()
    {
        contentHolder = new GameObject("Content");
        contentHolder.transform.SetParent(this.transform);
        contentHolder.transform.localPosition = Vector3.zero;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != contentHolder.transform)
            {
                child.SetParent(contentHolder.transform);
            }
        }
    }

    // ==========================================
    // LUỒNG 1: BẬT TẮT THEO KHOẢNG CÁCH
    // ==========================================
    private IEnumerator CheckDistanceRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= activationDistance && !isChunkActive)
                {
                    contentHolder.SetActive(true);
                    isChunkActive = true;
                }
                else if (distance > activationDistance && isChunkActive)
                {
                    contentHolder.SetActive(false);
                    isChunkActive = false;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    // ==========================================
    // LUỒNG 2: TỰ ĐỘNG ĐẺ QUÁI
    // ==========================================
    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            // Nghỉ một khoảng thời gian trước khi bắt đầu đợt mới
            yield return new WaitForSeconds(spawnInterval);

            // CHỈ đẻ quái khi Chunk đang hoạt động và có gán Prefab
            if (isChunkActive && enemyPrefabs != null && enemyPrefabs.Length > 0)
            {
                int currentEnemies = GetCurrentEnemyCount();

                // LƯU Ý LOGIC: Chỉ cần số lượng hiện tại < Tối đa thì cho phép đẻ
                // VD: max = 10, hiện tại = 9 -> Vẫn cho đẻ. Đẻ 2 con thành 11/10.
                if (currentEnemies < maxEnemies)
                {
                    for (int i = 0; i < spawnAmount; i++)
                    {
                        SpawnRandomEnemy();
                    }
                    // Debug để bạn dễ theo dõi dưới Console
                    // Debug.Log($"[Chunk] Đã sinh {spawnAmount} quái. Tổng số quái: {currentEnemies + spawnAmount}/{maxEnemies}");
                }
            }
        }
    }

    // Hàm đếm số lượng quái hiện tại đang sống trong Chunk
    private int GetCurrentEnemyCount()
    {
        int count = 0;
        // Duyệt qua tất cả các con của hộp chứa (Content)
        foreach (Transform child in contentHolder.transform)
        {
            // Bất kể là quái loại gì, miễn có tag "Enemy" và chưa bị Destroy thì đếm
            if (child.CompareTag("Enemy"))
            {
                count++;
            }
        }
        return count;
    }

    // Hàm thực thi việc đẻ 1 con quái
    private void SpawnRandomEnemy()
    {
        // 1. Chọn ngẫu nhiên 1 loại quái trong danh sách Prefab
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefabToSpawn = enemyPrefabs[randomIndex];

        // 2. Tính toán tọa độ (X ngẫu nhiên, Y cố định)
        float randomX = Random.Range(transform.position.x + minSpawnX, transform.position.x + maxSpawnX);
        Vector2 spawnPos = new Vector2(randomX, transform.position.y + fixedSpawnY);

        // 3. Đẻ ra quái và ném nó vào làm "Con" của ContentHolder để nó tự động tắt/bật theo Chunk
        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        newEnemy.transform.SetParent(contentHolder.transform);
    }

    // ==========================================
    // VẼ TRÊN EDITOR ĐỂ DỄ CĂN CHỈNH
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn Tầm hoạt động (Màu Vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);

        // Vẽ đường giới hạn Tọa độ Đẻ Quái (Màu Đỏ)
        Gizmos.color = Color.red;
        Vector3 leftPos = new Vector3(transform.position.x + minSpawnX, transform.position.y + fixedSpawnY, 0);
        Vector3 rightPos = new Vector3(transform.position.x + maxSpawnX, transform.position.y + fixedSpawnY, 0);

        Gizmos.DrawLine(leftPos, rightPos);
        Gizmos.DrawWireSphere(leftPos, 0.2f);
        Gizmos.DrawWireSphere(rightPos, 0.2f);
    }
}