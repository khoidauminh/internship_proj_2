using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button _unpause;
    [SerializeField] private Button _returnToMenu;
    [SerializeField] private Slider _volume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _unpause.onClick.AddListener(() =>
        {
            GameManager.GetInstance().Unpause();
        });

        _returnToMenu.onClick.AddListener(() => {
            GameManager.GetInstance().ReturnToMenu();
        });

        _volume.onValueChanged.AddListener((v) => AudioManager.GetInstance().ChangeVolume(v));
    }
}
