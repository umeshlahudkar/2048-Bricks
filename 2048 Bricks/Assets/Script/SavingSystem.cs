using UnityEngine;

public class SavingSystem : Singleton<SavingSystem>
{
    private string key = "GameData";

    protected override void Awake()
    {
        base.Awake();

        if (!PlayerPrefs.HasKey(key))
        {
            SaveData saveData = new();
            saveData.highScore = 0;

            Save(saveData);
        }
    }

    public void Save(SaveData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
    }

    public SaveData Load()
    {
        if(PlayerPrefs.HasKey(key))
        {
            string jsonData = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<SaveData>(jsonData);
        }
        return default;
    }

    public void Delete()
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}

public struct SaveData
{
    public int highScore;
}
