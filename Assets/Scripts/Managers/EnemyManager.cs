using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EnemyManager : MonoBehaviour
{
    class Instance
    {
        private List<BaseUnitConfig> _types;
        private List<int> _spawnCount;
        private readonly List<int> _spawnCountMax = new List<int>{10, 10, 10};

        private int _wave = 0;

        private float _spawnTimer;

        private List<BaseUnitController> _enemies = new List<BaseUnitController>();
        private int _enemiesKiled;

        private int _maxEnemy = 10;

        public Instance()
        {
            _enemiesKiled = 0;

            _types = new List<BaseUnitConfig>{};
            _spawnCount = new List<int>{0, 0, 0};

            BaseUnitConfig cylinder = ScriptableObject.CreateInstance<BaseUnitConfig>();
            cylinder.Initialize(5, 5, 2f, "Cylinder");

            BaseUnitConfig cube = ScriptableObject.CreateInstance<BaseUnitConfig>();
            cube.Initialize(1, 1, 5f, "Cube");

            BaseUnitConfig capsule = ScriptableObject.CreateInstance<BaseUnitConfig>();
            capsule.Initialize(1, 1, 2f, "Capsule");

            _types.Add(cylinder);
            _types.Add(cube);
            _types.Add(capsule);
        }

        public void SpawnEnemy()
        {
            BaseUnitConfig config = _types[_wave];

            if (_spawnCount[_wave] >= _spawnCountMax[_wave])
            {
                return;
            }

            Vector3 spawnPoint = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            GameObject enemy = UnitPool.Instance.GetUnit(config);

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

            _spawnCount[_wave] += 1;

            _enemies.Add(controller);
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
            if (_spawnCount[_wave] >= _spawnCountMax[_wave] && _enemies.Count == 0 && _wave < 3)
            {
                GameObject.Find("CameraHolder").GetComponent<CameraController>().Shake();
                ClearAll();
                
                _wave += 1;
                _spawnTimer = 2f;
                return;
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
            _spawnTimer -= Time.deltaTime;
            _enemiesKiled += _enemies.RemoveAll(enemy => !enemy.gameObject.activeSelf);
            if (_enemiesKiled > 0)
            {
                CheckLevel();
            }

            CheckSpawn();
            AdvanceCurrentEnemies();
        }
    }

    private static Instance _instance;

    void Awake()
    {
        _instance ??= new Instance();
    }

    void Update()
    {
        _instance.Update();
    }
}
