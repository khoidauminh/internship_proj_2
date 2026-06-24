using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemiesKilled;
    [SerializeField] private GameObject _enemiesKilledButton;
    [SerializeField] private Slider _playerHealth;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_Text _runText;
    [SerializeField] private Animator _animator;
    [SerializeField] private FixedJoystick _moveJoystick;
    [SerializeField] private FixedJoystick _turnJoystick;
    [SerializeField] private Button _attackButton;

    void Start()
    {
        GameManager game = GameManager.GetInstance();

        SetRunCount(game.RunCount);
        SetNewKill(game.CurrentSaveData.enemiesKilled);

        _backButton.onClick.AddListener(GameManager.GetInstance().HandlePause);
        _attackButton.onClick.AddListener(() =>
        {
            ColliderController collidercontroller = FindAnyObjectByType<ColliderController>();
            if (collidercontroller != null)
            {
                collidercontroller.HandleAttack();
            }
        });

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
        Vector2 moveVec = new Vector2(_moveJoystick.Horizontal, _moveJoystick.Vertical);
        float turnDelta = _turnJoystick.Horizontal * Time.deltaTime * 100f;

        GameManager game = GameManager.GetInstance();

        _playerHealth.value = (float) GameManager.GetInstance().PlayerHealth / (float)GameManager.GetInstance().GetConfig().PlayerMaxHealth;
        Debug.Log($"{_playerHealth.value}");

        if (moveVec.magnitude > 0.1 || Application.platform == RuntimePlatform.Android)
        {
            game.SetMoveDirection(moveVec.normalized);
        }

        game.SetTurnDelta(turnDelta);
    }
}
