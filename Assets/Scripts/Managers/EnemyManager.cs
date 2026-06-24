using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    private readonly List<int> _spawnCount = new List<int> { 0, 0, 0 };
    private DataManager.Config _config;

    private GameObject[] _spawnAreas;

    private float _spawnTimer;

    private List<BaseUnitController> _enemies;
    private int _enemiesKiled;

    private bool _cleared = false;

    public bool Cleared => _cleared;

    private const float SPAWN_DIST = 3f;
    private const float SPAWN_RANGE = 4f;

    void Awake()
    {
        _enemies = new();
    }

    void Start()
    {
        _enemiesKiled = GameManager.GetInstance().CurrentSaveData.enemiesKilled;
        _config = GameManager.GetInstance().GetConfig();
        _spawnAreas = GameObject.FindGameObjectsWithTag("EnemySpawn");

        Debug.Log($"Found {_spawnAreas.Length} spawn areas");
    }

    public int EnemiesKilled()
    {
        return _enemiesKiled;
    }

    private Vector3 computeSpawnPoint()
    {
        if (_spawnAreas == null)
        {
            Debug.LogWarning("Can't find Spawn Limiter");

            float spawnX = (Random.Range(0f, SPAWN_RANGE) + SPAWN_DIST) * (UnityEngine.Random.value < 0.5f ? 1f : -1f);
            float spawnY = (Random.Range(0f, SPAWN_RANGE) + SPAWN_DIST) * (UnityEngine.Random.value < 0.5f ? 1f : -1f);

            spawnX = Mathf.Clamp(spawnX, -10f, 10f);
            spawnY = Mathf.Clamp(spawnY, -10f, 10f);

            return new Vector3(spawnX, 0, spawnY);
        }

        var spawnArea = _spawnAreas[Random.Range(0, _spawnAreas.Length)];

        float Xlo = spawnArea.transform.position.x - spawnArea.transform.localScale.x / 2f;
        float Xhi = spawnArea.transform.position.x + spawnArea.transform.localScale.x / 2f;

        float Zlo = spawnArea.transform.position.z - spawnArea.transform.localScale.z / 2f;
        float Zhi = spawnArea.transform.position.z + spawnArea.transform.localScale.z / 2f;

        return new Vector3(Random.Range(Xlo, Xhi), 0, Random.Range(Zlo, Zhi));
    }

    public void SpawnEnemy()
    {
        int _level = GameManager.GetInstance().CurrentSaveData.level;

        if (_level >= _config.Levels.Count)
        {
            return;
        }

        BaseUnitConfig.Stats stats = _config.Levels[_level].Stats;

        if (_spawnCount[_level] >= _config.Levels[_level].NumEnemies)
        {
            return;
        }

        Vector3 spawnPoint = computeSpawnPoint();
        GameObject enemy = UnitPool.GetInstance().GetUnit(stats);

        if (enemy == null)
        {
            Debug.Log("Null Enemy returned from pool.");
            return;
        }

        BaseUnitController controller = enemy.GetComponent<BaseUnitController>();

        controller.Initialize(stats);
        controller.transform.Translate(spawnPoint);

        if (controller == null)
        {
            Debug.LogError("Spawned object does not have a BaseUnitController component.");
            Destroy(enemy);
            return;
        }

        _spawnCount[_level] += 1;
        _enemies.Add(controller);
        GameManager.GetInstance().BroadcastEnemySpawn(controller.transform.position);
    }

    public void Adopt(BaseUnitController e)
    {
        if (e == null)
        {
            return;
        }

        if (_enemies.Contains(e))
        {
            Debug.Log("Already in list");
            return;
        }

        Debug.Log("Added to list");

        _enemies.Add(e);
    }

    void AdvanceCurrentEnemies()
    {
        foreach (BaseUnitController enemy in _enemies)
        {
            enemy.CustomUpdate();
        }
    }

    void ClearAll()
    {
        foreach (BaseUnitController enemy in _enemies)
        {
            enemy.Die();
        }

        _enemies.Clear();
    }

    void CheckLevel()
    {
        Debug.Log("Checking...");

        int _level = GameManager.GetInstance().CurrentSaveData.level;

        if (_level < 3 && _spawnCount[_level] >= _config.Levels[_level].NumEnemies && _enemies.Count == 0)
        {
            GameObject player = GameObject.Find("Player");
            GameObject.Find("CameraHolder").GetComponent<CameraController>().Shake();
            ClearAll();

            Debug.Log("Level Up!");

            _level += 1;

            _cleared |= (_level == _config.Levels.Count);

            AudioManager.GetInstance().LevelUp(player.transform.position);
            GameManager.GetInstance().LevelUp(_level);

            if (_cleared)
            {
                AudioManager.GetInstance().StopAll();
                AudioManager.GetInstance().Win();
                Debug.Log("All levels cleared!");
            }

            _spawnTimer = 2f;
        }
    }

    void CheckSpawn()
    {
        if (_spawnTimer <= 0)
        {
            SpawnEnemy();
            _spawnTimer = Random.Range(0.5f, 1.5f);
        }
    }

    public void Update()
    {
        if (!GameManager.GetInstance().IsPlaying())
        {
            return;
        }

        _spawnTimer -= Time.deltaTime;

        int enemiesJustKilled = _enemies.RemoveAll(enemy => !enemy.gameObject.activeSelf);

        _enemiesKiled += enemiesJustKilled;

        if (enemiesJustKilled > 0)
        {
            GameManager.GetInstance().EnemyKilled(_enemiesKiled);
            CheckLevel();
        }

        CheckSpawn();
        AdvanceCurrentEnemies();
    }
}
