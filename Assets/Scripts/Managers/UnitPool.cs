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
        
    }

    private static UnitPool _instance;

    public static UnitPool GetInstance()
    {
        _instance ??= FindAnyObjectByType<UnitPool>();
        _instance ??= new GameObject(nameof(UnitPool)).AddComponent<UnitPool>();
        return _instance;
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

        Init();
    }

    class PoolElement
    {
        [SerializeField] private static readonly int initialPoolSize = 10;

        public Queue<GameObject> objects = new Queue<GameObject>();

        string prefabPath;

        public PoolElement(string prefabPath)
        {
            this.prefabPath = prefabPath;

            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject newObj = Instantiate(Resources.Load<GameObject>(prefabPath));
                newObj.SetActive(false);
                objects.Enqueue(newObj);
            }
        }
        private GameObject getOrNew()
        {
            //objects = new Queue<GameObject>(objects.Where(o => o == null));

            while (objects.Count > 0 && objects.Peek() == null) objects.Dequeue();

            if (objects.Count > 0)
            {
                if (!objects.Peek().activeSelf)
                {
                    return objects.Dequeue();
                }
            }

            return Instantiate(Resources.Load<GameObject>(prefabPath));
        }

        public GameObject Get()
        {
            objects = new Queue<GameObject>(objects.Where(o => o == null));
            GameObject obj = getOrNew();
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
            pools[config] = new PoolElement($"Prefabs/{config.Name}");
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

}
