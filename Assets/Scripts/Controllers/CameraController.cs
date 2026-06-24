using UnityEngine;
using static UnityEngine.UI.Image;

public class CameraController : MonoBehaviour
{
    private float _shakeTimer = 0f;
    private const float _shakeDuration = 0.5f;
    
    private Vector3 _cameraOffset;
    private float _distFactor;

    private readonly Vector3 cameraYoffset = new Vector3(0, 1f, 0);

    private int raycastLayers;

    private GameObject _camera;

    void Awake()
    {
       _camera = transform.GetChild(0).gameObject;
       raycastLayers = LayerMask.GetMask("World");
    }

    private void OnEnable()
    {
        GameManager.GetInstance().OnPlayerHurt += Shake;
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().OnPlayerHurt -= Shake;
    }

    public void Shake()
    {
        _shakeTimer = _shakeDuration;
    }
    
    public void CustomUpdate()
    {
        GameObject player = GameObject.Find("Player");
        GameManager game = GameManager.GetInstance();
        transform.position = Vector3.Lerp(transform.position, player.transform.position, 3f * Time.deltaTime);

        GameObject camera = transform.GetChild(0).gameObject;

        transform.Rotate(0, game.TurnDelta, 0);

        if (Physics.Raycast(player.transform.position, camera.transform.position - player.transform.position, out RaycastHit hit, 3f, raycastLayers))
        {
            _cameraOffset.z = -hit.distance;
        } else
        {
            _cameraOffset.z = -4f;
        }

        if (_shakeTimer > 0f)
        {
            _shakeTimer -= Time.deltaTime;
            float shakeX = Random.Range(-0.1f, 0.1f) * _shakeTimer / _shakeDuration;
            float shakeY = Random.Range(-0.1f, 0.1f) * _shakeTimer / _shakeDuration;
            camera.transform.localPosition = cameraYoffset + _cameraOffset + new Vector3(shakeX, 0, shakeY);
        }
        else
        {
            camera.transform.localPosition = cameraYoffset + _cameraOffset;
        }
    }
}
