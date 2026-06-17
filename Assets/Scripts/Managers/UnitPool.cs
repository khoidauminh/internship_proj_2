using UnityEngine;
using System.Collections.Generic;

public class UnitPool : MonoBehaviour
{
    public class UnitPoolInstance
    {
        public static GameObject _holder;

        public UnitPoolInstance(GameObject holder)
        {
            _holder = holder;
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
                    newObj.transform.SetParent(UnitPoolInstance._holder.transform);
                    newObj.SetActive(false);
                    objects.Enqueue(newObj);
                }
            }

            public void Enqueue(GameObject obj)
            {
                obj.SetActive(false);
                objects.Enqueue(obj);
            }

            public GameObject Dequeue()
            {
                if (objects.Count == 0)
                {
                    // Debug.LogWarning($"Pool for {prefab.name} is empty. Instantiating new object.");

                    GameObject newObj = Instantiate(prefab);
                    newObj.transform.SetParent(UnitPoolInstance._holder.transform);
                    newObj.SetActive(true);
                    return newObj;
                }

                // Debug.Log($"Dequeueing object from pool for {prefab.name}. Remaining objects: {objects.Count - 1}");

                GameObject obj = objects.Dequeue();
                obj.SetActive(true);

                return obj;
            }
        }

        Dictionary<BaseUnitConfig, PoolElement> pools = new Dictionary<BaseUnitConfig, PoolElement>();

        void InitializePools(List<BaseUnitConfig> configs)
        {
            foreach (var config in configs)
            {
                if (!pools.ContainsKey(config))
                {
                    pools[config] = new PoolElement(Resources.Load<GameObject>($"Prefabs/{config.Name}"));
                }
            }
        }

        public GameObject GetUnit(BaseUnitConfig config)
        {
            if (!pools.ContainsKey(config) || pools[config].objects.Count == 0)
            {
                InitializePools(new List<BaseUnitConfig> { config });
            }

            return pools[config].Dequeue();
        }

        public void ReleaseUnit(BaseUnitController unit)
        {
            if (unit == null || unit.Config == null)
            {
                return;
            }

            if (pools.ContainsKey(unit.Config))
            {
                pools[unit.Config].Enqueue(unit.gameObject);
            }
            else
            {
                Destroy(unit.gameObject);
            }
        }
    }

    private static UnitPoolInstance _instance;

    public static UnitPoolInstance Instance
    {
        get
        {
            _instance ??= new UnitPoolInstance(GameObject.Find("Enemy Holder"));
            return _instance;
        }
    }
}
