using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Cài đặt Rương")]
    public GameObject interactUI; // Nút bấm 'F'
    private bool isPlayerInRange = false;
    private bool isOpen = false;
    private Animator anim;

    [Header("Xử lý Loot đồ")]
    private InventoryManager inventory;
    public ItemData dropItem;
    public GameObject lootPrefab;
    [Range(0, 5)] public int dropCount = 1;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (interactUI != null) interactUI.SetActive(false);

        // Tìm InventoryManager giống hệt bên EnemyHealth
        if (GameManager.Instance != null)
        {
            inventory = GameManager.Instance.GetComponentInChildren<InventoryManager>();
        }
    }

    void Update()
    {
        if (isPlayerInRange && !isOpen && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isOpen = true;

        // 1. Tắt nút UI đi
        if (interactUI != null) interactUI.SetActive(false);

        // 2. Chạy Animation mở rương
        if (anim != null) anim.SetTrigger("Open");

        // 3. Gọi hàm rớt đồ (Bạn có thể bỏ dòng này đi nếu muốn gọi từ Animation Event giống con quái)
    }

    // Đổi thành public để nếu muốn, bạn có thể gọi từ Animation Event ở frame cuối cùng của rương
    public void DropLoot()
    {
        Debug.Log("Rương mở, bắt đầu rớt đồ...");
        // Logic rớt đồ sao chép y hệt OnAnimationDeadFinish
        if (dropItem != null && lootPrefab != null && inventory != null)
        {
            for (int i = 0; i < dropCount; i++)
            {
                // Cho đồ văng ra cao hơn tâm của rương một chút cho đẹp
                Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y + 0.5f);

                GameObject droppedLoot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);
                droppedLoot.GetComponent<SpriteRenderer>().sprite = dropItem.icon;

                // Ném thẳng vào túi đồ của Player
                inventory.AddItem(dropItem);
            }
        }
    }

    // ==========================================
    // NHẬN DIỆN NGƯỜI CHƠI RA VÀO VÙNG
    // ==========================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            isPlayerInRange = true;
            if (interactUI != null) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            isPlayerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}