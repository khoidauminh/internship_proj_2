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
    }

    private readonly string path = Path.Combine(Application.persistentDataPath, "save.json");
    private readonly string runCountPath = Path.Combine(Application.persistentDataPath, "runCount.txt");

    public static SaveData NewData()
    {
        return new SaveData();
    }

    public int GetRunCount()
    {
        try
        {
            return int.Parse(File.ReadAllText(runCountPath));
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to load run count at {runCountPath}: {e}");
            return 1;
        }
    }

    public void SaveRunCount(int count)
    {
        try
        {
            File.WriteAllText(runCountPath, count.ToString());
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to save run count {count}: {e}");
        }
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
