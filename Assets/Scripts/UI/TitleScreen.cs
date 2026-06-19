using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Button uiNew;
    [SerializeField] private Button uiLoad;
    [SerializeField] private Button uiSettings;
    [SerializeField] private Button uiExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiNew.onClick.AddListener(OnClickNewButton);
        uiLoad.onClick.AddListener(OnClickLoadButton);
        uiSettings.onClick.AddListener(OnClickSettingsButton);
        uiExit.onClick.AddListener(OnClickExitButton);
    }

    void OnClickNewButton()
    {
        Debug.Log("Clicked on New");
        GameManager game = GameManager.GetInstance();
        game.ResetData();
        game.SwitchToGameScene();
    }

    void OnClickLoadButton()
    {
        Debug.Log("Clicked on Load");
        GameManager game = GameManager.GetInstance();
        game.LoadData();
        game.SwitchToGameScene();
    }

    void OnClickSettingsButton()
    {

    }

    void OnClickExitButton()
    {

    }
}
