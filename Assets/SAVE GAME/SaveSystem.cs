using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string _savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _savePath = Application.persistentDataPath + "/savefile.json";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm Save nhận trực tiếp PlayerDataManager
    public void SaveGame(PlayerDataManager manager)
    {
        if (manager == null) return;

        // Ép dữ liệu cập nhật tọa độ mới nhất của nhân vật ngoài bản đồ
        manager.UpdateCurrentPosition();

        // Chuyển toàn bộ Manager thành chuỗi ký tự Json
        string json = JsonUtility.ToJson(manager, true);
        File.WriteAllText(_savePath, json);

        Debug.Log("Lưu game trực tiếp từ PlayerDataManager thành công!");
    }

    // Hàm Load ghi đè thẳng vào PlayerDataManager đang chạy
    public bool LoadGame(PlayerDataManager manager)
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);

            // KỸ THUẬT THEN CHỐT: Đè thẳng chuỗi văn bản Json vào Object manager đang sống trong RAM
            JsonUtility.FromJsonOverwrite(json, manager);

            Debug.Log("Tải file và đồng bộ hóa thành công vào PlayerDataManager!");
            return true;
        }

        Debug.LogError("Không tìm thấy file save để tải dữ liệu!");
        return false;
    }
}