using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [HideInInspector] public PlayerData PlayerData = new PlayerData();

    private string savePath;

    [SerializeField] private Texture2D _cursorTexture2D;
    public int MapIndex;


    [HideInInspector] public Data SavePlayerData = new Data();

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        PlayerDataManager[] playerDataManagers = FindObjectsOfType<PlayerDataManager>();
        if (playerDataManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Cursor.SetCursor(_cursorTexture2D, Vector2.zero, CursorMode.Auto);
    }

    public void SaveData()
    {
        if (PlayerData.PlayerType != 99)
        {
            SavePlayerData.datas[PlayerData.PlayerType] = PlayerData;
        }

        string Json = JsonUtility.ToJson(SavePlayerData, true);
        File.WriteAllText(savePath, Json);
    }

    public void DeleteData()
    {
        if (CheckData())
            File.Delete(savePath);
    }

    public bool CheckData()
    {
        return File.Exists(savePath);
    }

    public void LoadData(int index = 99)
    {
        if (File.Exists(savePath))
        {
            SavePlayerData = JsonUtility.FromJson<Data>(File.ReadAllText(savePath));
            if (index != 99)
            {
                PlayerData = SavePlayerData.datas[index];
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "Title" && SavePlayerData.PlayerName != "")
            SaveData();
    }
}