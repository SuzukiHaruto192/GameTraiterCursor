using UnityEngine;
using System.Collections;

public class BoDSpell : MonoBehaviour
{
    [Header("Cài đặt Mưa Phép (Spell)")]
    public GameObject spellPrefab;

    [Tooltip("Dịch chuyển độ cao của phép so với chân Player")]
    public float yOffset = -1.5f;

    public int spellCount = 5;
    public float spellSpacing = 2.5f; // Khoảng cách giữa các cột
    public float spellDelay = 0.15f;  // Tốc độ mọc chiêu (nên nhanh hơn để tạo áp lực)

    // Hàm nhận tọa độ Player và hướng nhìn của Boss
    public void StartSpellSequence(Transform playerTransform, float facingDirection)
    {
        StartCoroutine(SpawnSequenceRoutine(playerTransform, facingDirection));
    }

    private IEnumerator SpawnSequenceRoutine(Transform playerTransform, float facingDirection)
    {
        if (spellPrefab == null || playerTransform == null) yield break;

        // TÍNH TOÁN ĐIỂM BẮT ĐẦU DỰA TRÊN PLAYER
        // Chúng ta muốn chuỗi phép quét qua Player. 
        // Ví dụ: Bắt đầu từ phía sau Player (ngược hướng Boss nhìn) 
        // và tiến dần về phía trước (theo hướng Boss nhìn).

        float halfTotalLength = ((spellCount - 1) * spellSpacing) / 2f;

        // startX: Vị trí Player trừ đi một nửa tổng chiều dài chuỗi phép (nhân với hướng)
        // Điều này giúp Player luôn nằm ở trung tâm của chuỗi 5 cột phép.
        float startX = playerTransform.position.x - (facingDirection * halfTotalLength);
        float spawnY = playerTransform.position.y + yOffset;

        Vector2 startPos = new Vector2(startX, spawnY);

        for (int i = 0; i < spellCount; i++)
        {
            // Cột phép sẽ mọc dần theo hướng Boss nhìn (facingDirection)
            float newX = startPos.x + (facingDirection * spellSpacing * i);
            Vector2 spawnPos = new Vector2(newX, spawnY);

            Instantiate(spellPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spellDelay);
        }
    }
}