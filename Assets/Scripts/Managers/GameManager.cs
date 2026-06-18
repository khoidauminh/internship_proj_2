using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // internal EnemyManager _enemyManager;

    public enum Screen
    {
        Startup,
        Title,
        Game
    }

    private Screen _currentScreen;
    // private UIRoot _uiRoot;
    public event Action<Screen, Screen> OnScreenChange;

    private static GameManager _instance;
    private static Dictionary<Screen, string> _sceneNames;
    public static GameManager GetInstance()
    {
        _instance ??= FindAnyObjectByType<GameManager>();
        _instance ??= new GameObject(nameof(GameManager)).AddComponent<GameManager>();
        return _instance;
    }

    void Awake()
    {
        _currentScreen = Screen.Startup;
        _sceneNames = new Dictionary<Screen, string>
        {
            [Screen.Title] = "title",
            [Screen.Game] = "game"
        };
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

        ChangeScreen(Screen.Title);
    }

    public void ChangeScreen(Screen scr)
    {
        OnScreenChange?.Invoke(_currentScreen, scr);
        _currentScreen = scr;
    }

    public void SwitchToGameScene()
    {
        SceneManager.LoadScene(_sceneNames[Screen.Game]);
        ChangeScreen(Screen.Game);
    }

    public void Unpause()
    {
        // _currentScreen = Screen.Game;
        Time.timeScale = 1;
    }

    public void Pause()
    {
        // _currentScreen = Screen.Pause;
        Time.timeScale = 0;
    }

    void Update()
    {
    }
}
