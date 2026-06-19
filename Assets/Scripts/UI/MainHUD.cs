using UnityEngine;
using TMPro;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemiesKilled;
    [SerializeField] private GameObject _enemiesKilledButton;
    [SerializeField] private TMP_Text _runText;
    [SerializeField] private Animator _animator;

    void Start()
    {
        GameManager game = GameManager.GetInstance();

        SetRunCount(game.RunCount);
        SetNewKill(game.SaveData.enemiesKilled);

        game.OnEnemyKillCountChange += SetNewKill;
        game.OnNewRun += SetRunCount;
    }

    void SetRunCount(int count)
    {
        _runText.SetText("Run: {0}", count);
    }

    void SetNewKill(int count)
    {
        _enemiesKilled.SetText("Killed: {0}", count);
        _animator.SetTrigger("newKill");
    }
}
