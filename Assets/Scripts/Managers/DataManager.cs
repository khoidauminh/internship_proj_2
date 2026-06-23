using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class DataManager
{
    [System.Serializable]
    public class SaveData
    {
        public int enemiesKilled = 0;
        public int level = 0;
    }

    [System.Serializable]
    public class Config {

        [System.Serializable]
        public class Level
        {
            public int NumEnemies;

            public Level(int n)
            {
                NumEnemies = n;
            }
        }

        public List<Level> Levels = new List<Level> { new Level(10), new Level(10), new Level(15) };

        public override string ToString()
        {
            string ret = "\n";

            foreach (Level l in Levels)
            {
                ret += $"Level: {l.NumEnemies}\n";
            }

            return ret;
        }
    }

    private readonly string configPath = Path.Combine(Application.persistentDataPath, "config.json");
    private readonly string path = Path.Combine(Application.persistentDataPath, "save.json");
    private readonly string runCountPath = Path.Combine(Application.persistentDataPath, "runCount.txt");

    public static T New<T>() where T : class, new()
    {
        return new T();
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

    private string GetPathOf<T>() where T : class
    {
        if (typeof(T) == typeof(SaveData))
        {
            return path;
        }

        if (typeof(T) == typeof(Config))
        {
            return configPath;
        }

        Debug.LogError($"Invalid type {typeof(T)}");

        return null;
    }

    public T TryLoad<T>() where T : class, new()
    {
        try
        {
            string loadPath = GetPathOf<T>();

            string fileContent = File.ReadAllText(loadPath);
            T ret = JsonUtility.FromJson<T>(fileContent);
            Debug.Log($"Loaded {ret}");

            return ret;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
            T ret = new T();
            Debug.Log($"Created {ret}");
            return ret;
        }
    }

    public void TrySave<T>(T save) where T : class, new()
    {   
        try
        {
            string savePath = GetPathOf<T>();


            Debug.Log($"DEBUG SAVE: {save}");

            // Setting the second parameter to 'true' enables pretty printing (readable formatting)
            string jsonString = JsonUtility.ToJson(save, true);
            File.WriteAllText(savePath, jsonString);
            Debug.Log($"Data saved to: {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
        }
    }
}
