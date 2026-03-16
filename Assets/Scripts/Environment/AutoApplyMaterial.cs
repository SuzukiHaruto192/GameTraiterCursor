using UnityEngine;

public class AutoApplyMaterial : MonoBehaviour
{
    [Header("Kéo Material mờ vào đây")]
    public Material blurMaterial;

    // Nút thần kỳ để chạy ngay trong màn hình Edit (không cần ấn Play)
    [ContextMenu("Apply all child")]
    public void ApplyMaterialToAllChildren()
    {
        if (blurMaterial == null)
        {
            Debug.LogWarning("Ê, chưa kéo Material vào kìa!");
            return;
        }

        // Tìm tất cả Sprite Renderer trong cục này và các cục con
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.material = blurMaterial;
        }

        Debug.Log($"Đã ốp xong Material cho {renderers.Length} objects!");
    }
}