using UnityEngine;

public class EventBus : MonoBehaviour
{
    private static EventBus _instance;
    public static EventBus GetInstance()
    {
        _instance ??= FindAnyObjectByType<EventBus>();
        _instance ??= new GameObject(nameof(EventBus)).AddComponent<EventBus>();
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
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
