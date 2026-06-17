using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    class Instance
    {
        private List<BaseUnitConfig> _types = new List<BaseUnitConfig> { };
        private readonly List<int> _spawnCount = new List<int> { 0, 0, 0 };
        private readonly List<int> _spawnCountMax = new List<int> { 10, 10, 15 };

        private int _wave = 0;

        private float _spawnTimer;

        private GameObject _holder;
        private readonly List<BaseUnitController> _enemies = new List<BaseUnitController>();
        private int _enemiesKiled;

        private bool _cleared = false;

        public bool Cleared => _cleared;

        private const float SPAWN_DIST = 3f;
        private const float SPAWN_RANGE = 4f;

        public Instance(GameObject holder)
        {
            _enemiesKiled = 0;

            _holder = holder;

            BaseUnitConfig cylinder = ScriptableObject.CreateInstance<BaseUnitConfig>();
            cylinder.Initialize(5, 5, 2f, "Cylinder");

            BaseUnitConfig capsule = ScriptableObject.CreateInstance<BaseUnitConfig>();
            capsule.Initialize(1, 1, 2f, "Capsule");

            BaseUnitConfig cube = ScriptableObject.CreateInstance<BaseUnitConfig>();
            cube.Initialize(1, 1, 5f, "Cube");

            _types.Add(cylinder);
            _types.Add(capsule);
            _types.Add(cube);
        }

        public void SpawnEnemy()
        {
            if (_wave >= _types.Count)
            {
                return;
            }

            BaseUnitConfig config = _types[_wave];

            if (_spawnCount[_wave] >= _spawnCountMax[_wave])
            {
                return;
            }

            float spawnX = (Random.Range(0f, SPAWN_RANGE) + SPAWN_DIST) * (UnityEngine.Random.value < 0.5f ? 1f : -1f);
            float spawnY = (Random.Range(0f, SPAWN_RANGE) + SPAWN_DIST) * (UnityEngine.Random.value < 0.5f ? 1f : -1f);

            spawnX = Mathf.Clamp(spawnX, -10f, 10f);
            spawnY = Mathf.Clamp(spawnY, -10f, 10f);

            Vector3 spawnPoint = new Vector3(spawnX, 0, spawnY);
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

            ParticleManager.GetInstance().BurstEnemySpawnParticle(controller.transform.position);
            controller.gameObject.transform.SetParent(_holder.transform);
            AudioManager.Spawn(controller.transform.position);
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
            Debug.Log("Checking...");

            if (_wave < 3 && _spawnCount[_wave] >= _spawnCountMax[_wave] && _enemies.Count == 0)
            {
                GameObject player = GameObject.Find("Player");
                GameObject.Find("CameraHolder").GetComponent<CameraController>().Shake();
                ClearAll();

                AudioManager.LevelUp(player.transform.position);

                Debug.Log("Level Up!");

                _wave += 1;

                _cleared |= (_wave == 3);


                if (_cleared)
                {
                    AudioManager.StopAll();
                    AudioManager.Win();
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
            _spawnTimer -= Time.deltaTime;

            int enemiesJustKilled = _enemies.RemoveAll(enemy => !enemy.gameObject.activeSelf);

            _enemiesKiled += enemiesJustKilled;

            if (enemiesJustKilled > 0)
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
        _instance ??= new Instance(GameObject.Find("Enemy Holder"));
    }

    public void CustomUpdate()
    {
        _instance.Update();
    }
}
