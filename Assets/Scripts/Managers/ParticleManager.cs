using UnityEngine;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager _instance;

    private GameObject _particleHolder;

    public static ParticleManager GetInstance()
    {
        _instance ??= FindAnyObjectByType<ParticleManager>();
        _instance ??= new GameObject(nameof(ParticleManager)).AddComponent<ParticleManager>();

        return _instance;
    }

    public enum ParticleType
    {
        EnemySpawn,
        PlayerAttack
    }

    private GameObject _prefabSpawn;
    private GameObject _prefabAttack;

    private static Dictionary<ParticleType, Stack<GameObject>> _pools;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _pools = new Dictionary<ParticleType, Stack<GameObject>>
        {
            [ParticleType.EnemySpawn] = new Stack<GameObject> { },
            [ParticleType.PlayerAttack] = new Stack<GameObject> { },
        };

        _particleHolder = GameObject.Find("Particle Holder");

        GameObject prefabSpawn = Resources.Load<GameObject>("Prefabs/Particles/Spawn");
        GameObject prefabAttack = Resources.Load<GameObject>("Prefabs/Particles/Attack");

        // prefabSpawn.transform.SetParent(_particleHolder.transform);
        // prefabAttack.transform.SetParent(_particleHolder.transform);

        _pools[ParticleType.EnemySpawn].Push(prefabSpawn);
        _pools[ParticleType.PlayerAttack].Push(prefabAttack);
    }

    void SpawnParticle(ParticleType type, Vector3 position, Quaternion rotation, float life, Vector3 veclocity)
    {
        Stack<GameObject> pool = _pools[type];

        if (pool.Count <= 1)
        {
            GameObject newParticle = Instantiate(pool.Peek(), position, rotation);
            newParticle.transform.SetParent(_particleHolder.transform);
            newParticle.GetComponent<Rigidbody>().AddForce(veclocity);
            StartCoroutine(DisableThisIn(newParticle, type, life));
            return;
        }

        GameObject particle = pool.Pop();
        particle.transform.position = position;
        particle.transform.rotation = rotation;
        particle.transform.SetParent(_particleHolder.transform);
        particle.SetActive(true);
        particle.GetComponent<Rigidbody>().AddForce(veclocity);
        StartCoroutine(DisableThisIn(particle, type, life));
    }

    IEnumerator<WaitForSeconds> DisableThisIn(GameObject obj, ParticleType type, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
        _pools[type].Push(obj);
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

    public void BurstAttackParticle(Vector3 position, Vector3 source)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 veclocity = Vector3.Lerp((position - source).normalized, Random.onUnitSphere, 0.4f) * 15f;
            Quaternion rotation = Quaternion.LookRotation(veclocity - source);
            SpawnParticle(ParticleType.PlayerAttack, position, rotation, Random.Range(0.2f, 0.4f), veclocity);
        }
    }
}
