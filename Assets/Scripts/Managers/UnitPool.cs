using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitPool : MonoBehaviour
{
    Dictionary<BaseUnitConfig.Stats, PoolElement> pools;

    void Init()
    {
        pools = new Dictionary<BaseUnitConfig.Stats, PoolElement>();
    }

    void Awake()
    {
        Init();
    }

    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    class PoolElement
    {
        [SerializeField] private static readonly int initialPoolSize = 10;

        public GameObject prefab;

        public Queue<GameObject> objects = new Queue<GameObject>();

        public PoolElement(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null!!");
            }

            this.prefab = prefab;

            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject newObj = Instantiate(prefab);
                newObj.SetActive(false);
                objects.Enqueue(newObj);
            }
        }

        public GameObject Get()
        {
            GameObject obj = (objects.Count > 0 && !objects.Peek().activeSelf) ? objects.Dequeue() : Instantiate(prefab);
            obj.SetActive(true);
            return obj;
        }

        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            objects.Enqueue(obj);
        }
    }

    void PreInitializePoolIfNotSet(BaseUnitConfig.Stats config)
    {
        pools ??= new();

        if (!pools.ContainsKey(config))
        {
            pools[config] = new PoolElement(Resources.Load<GameObject>($"Prefabs/{config.Name}"));
        }
    }

    public GameObject GetUnit(BaseUnitConfig.Stats config)
    {
        PreInitializePoolIfNotSet(config);
        return pools[config].Get();
    }

    public void ReleaseUnit(BaseUnitController controller)
    {
        controller.gameObject.SetActive(false);
        pools[controller.Stats].Push(controller.gameObject);
    }

    private static UnitPool _instance;

    public static UnitPool GetInstance()
    {
        _instance ??= FindAnyObjectByType<UnitPool>();
        _instance ??= new GameObject(nameof(UnitPool)).AddComponent<UnitPool>();
        return _instance;
    }
}
