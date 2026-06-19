using UnityEngine;
using System;
using System.IO;

public class DataManager
{
    [Serializable]
    public class SaveData
    {
        public int enemiesKilled = 0;
        public int level = 0;
        public int run = 0;
    }

    private readonly string path = Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveData NewData()
    {
        return new SaveData();
    }

    public SaveData TryLoadData()
    {
        try
        {
            string fileContent = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(fileContent);
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
            return new SaveData();
        }
    }

    public void TrySaveData(SaveData save)
    {
        try
        {
            // Setting the second parameter to 'true' enables pretty printing (readable formatting)
            string jsonString = JsonUtility.ToJson(save, true);
            File.WriteAllText(path, jsonString);
            Debug.Log($"Data saved to: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
        }
    }
}
