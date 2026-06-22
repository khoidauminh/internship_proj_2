using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRoot : MonoBehaviour
{
    private static UIRoot instance;

    [SerializeField] private GameObject mainCanvas;

    Dictionary<string, MonoBehaviour> uiMap;

    public static UIRoot GetInstance()
    {
        instance ??= FindAnyObjectByType<UIRoot>();
        instance ??= new GameObject(nameof(UIRoot)).AddComponent<UIRoot>();
        return instance;
    }

    public static T LoadUIPrefab<T>(string name) where T : MonoBehaviour
    {
        string fullPath = "Prefabs/UI/" + name;
        GameObject prefab = Resources.Load<GameObject>(fullPath);

        if (prefab == null)
        {
            Debug.LogError($"Failed to load UI prefab at {fullPath}");
            return null;
        }

        T ui = Instantiate(prefab).GetComponent<T>();

        if (ui == null)
        {
            Debug.LogError($"Invalid prefab: {typeof(T)}");
            return null;
        }

        return ui;
    }

    public void ToggleUI(string name, bool active)
    {
        if (!uiMap.ContainsKey(name))
        {
            MonoBehaviour ui = LoadUIPrefab<MonoBehaviour>(name);
            uiMap[name] = ui;
            ui.gameObject.SetActive(active);
            ui.transform.SetParent(mainCanvas.transform, false);
        }

        uiMap[name].gameObject.SetActive(active);
        Debug.Log("UI Toggled");
    }

    void HandleSceneChange(string prev, string next)
    {
        if (prev == "title")
        {
            ToggleUI("Title Screen", false);
        }

        if (next == "title")
        {
            ToggleUI("Title Screen", true);
        }

        if (prev == "game")
        {
            ToggleUI("MainHUD", false);
        }

        if (next == "game")
        {
            ToggleUI("MainHUD", true);
        }
    }

    void HandlePauses(bool isPaused)
    {
        ToggleUI("Pause Screen", isPaused);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        uiMap = new();

        GameManager game = GameManager.GetInstance();

        ToggleUI("MainHUD", false);
        ToggleUI("Title Screen", true);
        ToggleUI("Pause Screen", false);

        game.OnSceneChange += HandleSceneChange;
        game.OnPause += HandlePauses;
    }
}
