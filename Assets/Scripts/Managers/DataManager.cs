using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static DataManager.Config;

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
            public BaseUnitConfig.Stats Stats;

            public Level(int n, BaseUnitConfig.Stats fg)
            {
                NumEnemies = n;
                Stats = fg;
            }
        }

        public int PlayerMaxHealth;
        public List<Level> Levels;

        public Config(int playerMaxHealth, List<Level> levels)
        {
            PlayerMaxHealth = playerMaxHealth;
            Levels = levels;
        }

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

    private Config defaultConfig;

    void Awake()
    {
        defaultConfig = new Config(3, new List<Level> {
            new Level(10, Resources.Load<BaseUnitConfig>("Configs/Cylinder").Get()),
            new Level(10, Resources.Load<BaseUnitConfig>("Configs/Capsule").Get()),
            new Level(15, Resources.Load<BaseUnitConfig>("Configs/Cube").Get())
        });
    }

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

    public Config TryLoadConfig()
    {
        try
        {
            string fileContent = File.ReadAllText(configPath);
            Config ret = JsonUtility.FromJson<Config>(fileContent);
            return ret;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
            Config ret = defaultConfig;
            Debug.Log($"Created {ret}");
            return ret;
        }
    }

    public void TrySaveConfig(Config config)
    {
        try
        {
            string jsonString = JsonUtility.ToJson(config, true);
            File.WriteAllText(configPath, jsonString);
            Debug.Log($"Data saved to: {configPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
        }
    }

    public SaveData TryLoadSaveData()
    {
        try
        {
            string fileContent = File.ReadAllText(path);
            SaveData ret = JsonUtility.FromJson<SaveData>(fileContent);
            Debug.Log($"Loaded {ret}");

            return ret;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred: {e}");
            SaveData ret = new SaveData();
            Debug.Log($"Created {ret}");
            return ret;
        }
    }

    public void TrySaveSaveData(SaveData save)
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
