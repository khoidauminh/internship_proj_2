using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    public enum ParticleType
    {
        EnemySpawn,
        PlayerAttack
    }

    private GameObject _prefabSpawn;
    private GameObject _prefabAttack;

    private Dictionary<ParticleType, Queue<Particle>> _pools;

    void Start()
    {
        GameManager.GetInstance().OnEnemySpawn += HandleSpawn;
        GameManager.GetInstance().OnPlayerKill += BurstAttackParticle;

        _prefabSpawn = Resources.Load<GameObject>("Prefabs/Particles/Spawn");
        _prefabAttack = Resources.Load<GameObject>("Prefabs/Particles/Attack");
    }

    void OnDestroy()
    {
        GameManager.GetInstance().OnEnemySpawn -= HandleSpawn;
        GameManager.GetInstance().OnPlayerKill -= BurstAttackParticle;
    }

    void HandleSpawn(Vector3 pos)
    {
        BurstEnemySpawnParticle(pos);
        AudioManager.GetInstance().Spawn(pos);
    }

    Particle InstantiateParticle(ParticleType type)
    {
        return Instantiate(type == ParticleType.EnemySpawn ? _prefabSpawn : _prefabAttack).GetComponent<Particle>();
    }

    void InitializePoolIfNotSet(ParticleType type)
    {
        _pools ??= new();

        if (!_pools.ContainsKey(type))
        {
            _pools[type] = new Queue<Particle> { };
        }

        Queue<Particle> pool = _pools[type];

        if (pool.Count == 0)
        {
            Particle particle = InstantiateParticle(type);
            particle.Init(type);
            _pools[type].Enqueue(particle);
        }
    }

    void SpawnParticle(ParticleType type, Vector3 position, Quaternion rotation, float life, Vector3 veclocity)
    {
        InitializePoolIfNotSet(type);

        Queue<Particle> pool = _pools[type];

        Particle particle = (pool.Count == 0 ? InstantiateParticle(type) : pool.Dequeue());
        particle.Init(type, life, veclocity, 0.99f);
        particle.transform.position = position;
        particle.gameObject.SetActive(true);
    }

    public void BurstEnemySpawnParticle(Vector3 position)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 veclocity = Random.onUnitSphere * 3f;
            Quaternion rotation = Quaternion.LookRotation(veclocity) * Quaternion.Euler(90f, 0, 0);
            SpawnParticle(ParticleType.EnemySpawn, position, rotation, Random.Range(0.5f, 1f), veclocity);
        }
    }

    public void BurstAttackParticle(Vector3 player, Vector3 enemy)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 veclocity = Vector3.Lerp((enemy - player).normalized, Random.onUnitSphere, 0.4f) * 15f;
            Quaternion rotation = Quaternion.LookRotation(veclocity - player);
            SpawnParticle(ParticleType.PlayerAttack, enemy, rotation, Random.Range(0.2f, 0.4f), veclocity);
        }
    }

    void Update()
    {
        Particle[] active = Object.FindObjectsByType<Particle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Particle p in active)
        {
            if (p.Decay())
            {
                p.gameObject.SetActive(false);
                _pools[p.Type].Enqueue(p);
            }
        }
    }
}
