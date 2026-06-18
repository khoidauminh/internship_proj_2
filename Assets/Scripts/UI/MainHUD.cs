using UnityEngine;
using TMPro;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemiesKilled;
    [SerializeField] private GameObject _enemiesKilledButton;
    [SerializeField] private Animator _animator;

    void Start()
    {
        GameManager.GetInstance().OnEnemyKillCountChange += (count) =>
        {
            _enemiesKilled.SetText("Killed: {0}", count);
            _animator.SetTrigger("newKill");
        };
    }
}
