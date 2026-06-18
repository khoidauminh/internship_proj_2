using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private string _currentScene;

    public event Action<string, string> OnSceneChange;
    public event Action<bool> OnPause;
    public event Action<int> OnEnemyKillCountChange;
    public event Action OnLevelUp;

    public void EnemyKilled(int enemiesKiled)
    {
        OnEnemyKillCountChange?.Invoke(enemiesKiled);
    }

    public void LevelUp()
    {
        OnLevelUp?.Invoke();
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

        ChangeScreen(SceneManager.GetActiveScene().name);
    }

    public void ChangeScreen(string newSceneName)
    {
        Debug.Log($"Changing scene from {_currentScene} to {newSceneName}");
        OnSceneChange?.Invoke(_currentScene, newSceneName);
        _currentScene = newSceneName;
    }

    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("game");
        ChangeScreen("game");
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

    void Update()
    {
    }
}
