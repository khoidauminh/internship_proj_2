using UnityEngine;

public class GameManager : MonoBehaviour
{
    class Instance
    {
        [SerializeField] private BaseUnitConfig _playerConfig;

        public Instance()
        {}
    }

    private static Instance _instance;

    void Awake()
    {
        _instance ??= new Instance();
    }
}
