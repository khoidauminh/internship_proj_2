using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BaseUnitConfig _playerConfig;

    internal PlayerController _playerController;
    internal EnemyManager _enemyManager;

    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        _instance ??= FindAnyObjectByType<GameManager>();
        _instance ??= new GameObject(nameof(GameManager)).AddComponent<GameManager>();
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

        _playerController = FindAnyObjectByType<PlayerController>();
        _enemyManager = FindAnyObjectByType<EnemyManager>();
    }

    void Update()
    {
        GetInstance()._playerController.CustomUpdate();
        GetInstance()._enemyManager.CustomUpdate();
    }
}
