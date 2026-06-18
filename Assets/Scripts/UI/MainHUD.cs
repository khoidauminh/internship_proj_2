using UnityEngine;
using TMPro;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemiesKilled;

    void Start()
    {
        EnemyManager.GetInstance().OnEnemyKillCountChange += (count) =>
        {
            _enemiesKilled.SetText("Killed: {0}", count);
        };
    }
}
