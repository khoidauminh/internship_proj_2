using UnityEngine;

public class GameManager : MonoBehaviour
{
    class Instance
    {
        [SerializeField] private BaseUnitConfig _playerConfig;

        public PlayerController playerController = FindAnyObjectByType<PlayerController>();
        public EnemyManager enemyManager = FindAnyObjectByType<EnemyManager>();
    }

    private static Instance _instance;

    void Awake()
    {
        _instance ??= new Instance();
    }

    void Update()
    {
        _instance.playerController.CustomUpdate();
        _instance.enemyManager.CustomUpdate();
    }
}
