using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemiesKilled;
    [SerializeField] private GameObject _enemiesKilledButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_Text _runText;
    [SerializeField] private Animator _animator;
    [SerializeField] private FixedJoystick _joystick;

    void Start()
    {
        GameManager game = GameManager.GetInstance();

        SetRunCount(game.RunCount);
        SetNewKill(game.SaveData.enemiesKilled);

        _backButton.onClick.AddListener(GameManager.GetInstance().HandlePause);
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

    void Update()
    {
        Vector2 vec = new Vector2(_joystick.Horizontal, _joystick.Vertical);

        if (vec.magnitude > 0.1 || Application.platform == RuntimePlatform.Android)
        {
            GameManager.GetInstance().SetMoveDirection(vec.normalized);
        }
    }
}
