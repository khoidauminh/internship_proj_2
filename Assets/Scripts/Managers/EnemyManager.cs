using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    private List<BaseUnitConfig> _types = new();
    private readonly List<int> _spawnCount = new List<int> { 0, 0, 0 };
    private DataManager.Config _config;

    private int _level = 0;

    private float _spawnTimer;

    private readonly List<BaseUnitController> _enemies = new();
    private int _enemiesKiled;

    private bool _cleared = false;

    public bool Cleared => _cleared;

    private const float SPAWN_DIST = 3f;
    private const float SPAWN_RANGE = 4f;

    void Start()
    {
        BaseUnitConfig cylinder = ScriptableObject.CreateInstance<BaseUnitConfig>();
        cylinder.Initialize(5, 5, 2f, "Cylinder");

        BaseUnitConfig capsule = ScriptableObject.CreateInstance<BaseUnitConfig>();
        capsule.Initialize(1, 1, 2f, "Capsule");

        BaseUnitConfig cube = ScriptableObject.CreateInstance<BaseUnitConfig>();
        cube.Initialize(1, 1, 5f, "Cube");

        _level = GameManager.GetInstance().CurrentSaveData.level;
        _enemiesKiled = GameManager.GetInstance().CurrentSaveData.enemiesKilled;
        _config = GameManager.GetInstance().GetConfig();

        _types.Add(cylinder);
        _types.Add(capsule);
        _types.Add(cube);
    }

    public int EnemiesKilled()
    {
        return _enemiesKiled;
    }

    public void SpawnEnemy()
    {
        if (_level >= _types.Count)
        {
            return;
        }

        BaseUnitConfig config = _types[_level];

        if (_spawnCount[_level] >= _config.Levels[_level].NumEnemies)
        {
            return;
        }

        float spawnX = (Random.Range(0f, SPAWN_RANGE) + SPAWN_DIST) * (UnityEngine.Random.value < 0.5f ? 1f : -1f);
        float spawnY = (Random.Range(0f, SPAWN_RANGE) + SPAWN_DIST) * (UnityEngine.Random.value < 0.5f ? 1f : -1f);

        spawnX = Mathf.Clamp(spawnX, -10f, 10f);
        spawnY = Mathf.Clamp(spawnY, -10f, 10f);

        Vector3 spawnPoint = new Vector3(spawnX, 0, spawnY);
        GameObject enemy = UnitPool.GetInstance().GetUnit(config);

        if (enemy == null)
        {
            Debug.Log("Null Enemy returned from pool.");
            return;
        }

        BaseUnitController controller = enemy.GetComponent<BaseUnitController>();

        controller.Initialize(config);
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

    void AdvanceCurrentEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].CustomUpdate();
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

        if (_level < 3 && _spawnCount[_level] >= _config.Levels[_level].NumEnemies && _enemies.Count == 0)
        {
            GameObject player = GameObject.Find("Player");
            GameObject.Find("CameraHolder").GetComponent<CameraController>().Shake();
            ClearAll();

            Debug.Log("Level Up!");

            _level += 1;

            _cleared |= (_level == 3);

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
