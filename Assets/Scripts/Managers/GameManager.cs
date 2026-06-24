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
    public float TurnDelta { get; private set; }
    public int PlayerHealth { get; private set; }

    public event Action<string, string> OnSceneChange;
    public event Action<bool> OnPause;
    public event Action<int> OnEnemyKillCountChange;
    public event Action<Vector3> OnEnemySpawn;
    public event Action<int> OnLevelUp;
    public event Action OnPlayerHurt;

    public event Action<int> OnNewRun;

    public event Action<Vector3, Vector3> OnPlayerKill;
    public event Action<Vector3> OnPlayerAttack;

    private DataManager _dataManager;

    [SerializeField]
    private DataManager.SaveData _currentSave;

    [SerializeField]
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
    
    public void BroadCastPlayerHurt()
    {
        PlayerHealth -= 1;

        if (PlayerHealth < 1)
        {
            SwitchToGameScene();
            return;
        }

        AudioManager.GetInstance().Hurt();
        OnPlayerHurt?.Invoke();
    }

    public void SetMoveDirection(Vector2 moveDir)
    {
        _moveDirection = moveDir;
        //Debug.Log($"{moveDir}, {moveDir}");
    }

    public void SetTurnDelta(float turn)
    {
        TurnDelta = turn;
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
        PlayerHealth = _currentConfig.PlayerMaxHealth;
        // _currentConfig = _dataManager.TryLoadConfig();

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
        Resources.UnloadUnusedAssets();
        _currentScene = newSceneName;
    }

    public void ResetData()
    {
        _currentSave = DataManager.New<DataManager.SaveData>();
    }

    public void LoadData()
    {
        _currentSave = _dataManager.TryLoadSaveData();
        Debug.Log($"Data: {_currentSave}");
        EnemyKilled(_currentSave.enemiesKilled);
    }

    void lockCursor(bool b)
    {
        Cursor.lockState = b ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !b;
    }

    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("game");
        ChangeScreen("game");
        EnemyKilled(_currentSave.enemiesKilled);
        OnNewRun?.Invoke(_runCount);
        _runCount += 1;

        Unpause();

        PlayerHealth = 3;
        //lockCursor(true);
    }

    public void ReturnToMenu()
    {
        _dataManager.TrySaveSaveData(_currentSave);
        _dataManager.TrySaveConfig(_currentConfig);
        _dataManager.SaveRunCount(_runCount);
        SceneManager.LoadScene("title");
        ChangeScreen("title");

        //lockCursor(false);
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

        //lockCursor(true);
    }

    public void Pause()
    {
        OnPause?.Invoke(true);
        Time.timeScale = 0;
        _isPaused = true;

        //lockCursor(false);
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
        if (!IsPaused())
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                _moveDirection = Input.Player.Move.ReadValue<Vector2>();
            }
        } 
        else
        {
            TurnDelta = 0f;
        }
    }
}
