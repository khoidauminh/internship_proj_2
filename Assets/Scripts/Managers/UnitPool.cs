using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitPool : MonoBehaviour
{
    public static GameObject _holder;

    void Awake()
    {
        _holder = GameObject.Find("Enemy Holder");
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
            DontDestroyOnLoad(gameObject);
        }
    }

    class PoolElement
    {
        [SerializeField] private static readonly int initialPoolSize = 10;

        public GameObject prefab;

        public Queue<GameObject> objects = new Queue<GameObject>();

        public PoolElement(GameObject prefab)
        {
            this.prefab = prefab;

            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject newObj = Instantiate(prefab);
                newObj.transform.SetParent(_holder.transform);
                newObj.SetActive(false);
                objects.Enqueue(newObj);
            }
        }

        public GameObject Get()
        {
            // objects = new Queue<GameObject>(objects.Where(obj => obj == null));
            GameObject obj = (objects.Count > 0 && !objects.Peek().activeSelf) ? objects.Dequeue() : Instantiate(prefab);
            obj.SetActive(true);
            obj.transform.SetParent(_holder.transform);
            return obj;
        }

        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            objects.Enqueue(obj);
        }
    }

    Dictionary<BaseUnitConfig, PoolElement> pools = new Dictionary<BaseUnitConfig, PoolElement>();

    void PreInitializePoolIfNotSet(BaseUnitConfig config)
    {
        if (!pools.ContainsKey(config))
        {
            pools[config] = new PoolElement(Resources.Load<GameObject>($"Prefabs/{config.Name}"));
        }
    }

    public GameObject GetUnit(BaseUnitConfig config)
    {
        PreInitializePoolIfNotSet(config);
        return pools[config].Get();
    }

    public void ReleaseUnit(BaseUnitController controller)
    {
        controller.gameObject.SetActive(false);
        pools[controller.Config].Push(controller.gameObject);
    }

    private static UnitPool _instance;

    public static UnitPool GetInstance()
    {
        _instance ??= FindAnyObjectByType<UnitPool>();
        _instance ??= new GameObject(nameof(UnitPool)).AddComponent<UnitPool>();
        return _instance;
    }
}
