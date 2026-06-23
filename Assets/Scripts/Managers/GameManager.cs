using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private string _currentScene;

    public PlayerInput Input { get; private set; }
    private Vector2 _moveDirection;

    public event Action<string, string> OnSceneChange;
    public event Action<bool> OnPause;
    public event Action<int> OnEnemyKillCountChange;
    public event Action<Vector3> OnEnemySpawn;
    public event Action<int> OnLevelUp;

    public event Action<int> OnNewRun;

    public event Action<Vector3, Vector3> OnPlayerKill;
    public event Action<Vector3> OnPlayerAttack;

    private DataManager _dataManager;
    private DataManager.SaveData _currentSave;
    private DataManager.Config _currentConfig;

    private int _runCount;
    public int RunCount => _runCount;
    public DataManager.SaveData CurrentSaveData => _currentSave;
    public DataManager.Config GetConfig() { return _currentConfig; }

    public void BroadCastPlayerKill(Vector3 player, Vector3 enemy)
    {
        OnPlayerKill?.Invoke(player, enemy);
        AudioManager.GetInstance().Explode(enemy);
    }

    public void BroadcastPlayerAttack(Vector3 pos)
    {
        OnPlayerAttack?.Invoke(pos);
        AudioManager.GetInstance().Smack(pos);
    }

    public void BroadcastEnemySpawn(Vector3 pos)
    {
        OnEnemySpawn?.Invoke(pos);
        AudioManager.GetInstance().Spawn(pos);
    }

    public void SetMoveDirection(Vector2 moveDir)
    {
        _moveDirection = moveDir;
        Debug.Log($"{moveDir}, {moveDir}");
    }

    public Vector2 MoveDirection()
    {
        return _moveDirection;
    }

    public void EnemyKilled(int enemiesKiled)
    {
        OnEnemyKillCountChange?.Invoke(enemiesKiled);
    }

    public void LevelUp(int newLevel)
    {
        _currentSave.enemiesKilled = FindAnyObjectByType<EnemyManager>().EnemiesKilled();
        _currentSave.level = newLevel;
        OnLevelUp?.Invoke(newLevel);
    }

    private bool _isPaused;

    private static GameManager _instance;

    public static GameManager GetInstance()
    {
        _instance ??= FindAnyObjectByType<GameManager>();
        _instance ??= new GameObject(nameof(GameManager)).AddComponent<GameManager>();
        return _instance;
    }

    void Awake()
    {
        _currentScene = "title";
        Input = new PlayerInput();
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

        _isPaused = false;
        _dataManager = new DataManager();
        _runCount = _dataManager.GetRunCount();
        _currentConfig = _dataManager.TryLoad<DataManager.Config>();

        Input.Player.Pause.performed += HandlePause;

        LoadData();

        ChangeScreen(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        Input.Player.Pause.performed -= HandlePause;
    }

    public void HandlePause(InputAction.CallbackContext ctx)
    {
        if (IsPaused()) Unpause(); else Pause();
    }

    public void HandlePause()
    {
        if (IsPaused()) Unpause(); else Pause();
    }

    public void ChangeScreen(string newSceneName)
    {
        Debug.Log($"Changing scene from {_currentScene} to {newSceneName}");
        OnSceneChange?.Invoke(_currentScene, newSceneName);
        _currentScene = newSceneName;
    }

    public void ResetData()
    {
        _currentSave = DataManager.New<DataManager.SaveData>();
    }

    public void LoadData()
    {
        _currentSave = _dataManager.TryLoad<DataManager.SaveData>();
        Debug.Log($"Data: {_currentSave}");
        EnemyKilled(_currentSave.enemiesKilled);
    }

    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("game");
        ChangeScreen("game");
        EnemyKilled(_currentSave.enemiesKilled);
        OnNewRun?.Invoke(_runCount);
        _runCount += 1;
    }

    public void ReturnToMenu()
    {
        _dataManager.TrySave<DataManager.SaveData>(_currentSave);
        _dataManager.SaveRunCount(_runCount);
        _dataManager.TrySave<DataManager.Config>(_currentConfig);
        SceneManager.LoadScene("title");
        ChangeScreen("title");
        Unpause();
    }

    public bool IsPlaying()
    {
        return !IsPaused() && _currentScene == "game";
    }

    public void Unpause()
    {
        OnPause?.Invoke(false);
        Time.timeScale = 1;
        _isPaused = false;
    }

    public void Pause()
    {
        OnPause?.Invoke(true);
        Time.timeScale = 0;
        _isPaused = true;
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    void OnEnable()
    {
        Input.Enable();
    }

    void OnDisable()
    {
        Input.Disable();
    }

    void Update()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            _moveDirection = Input.Player.Move.ReadValue<Vector2>();
        }
    }
}
