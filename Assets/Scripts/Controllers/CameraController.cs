using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float _shakeTimer = 0f;
    private const float _shakeDuration = 0.5f;
    
    private readonly Vector3 CAMERA_OFFSET = new Vector3(0, 4.82f, -7.22f);

    public void Shake()
    {
        _shakeTimer = _shakeDuration;
    }
    
    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("Player");
        transform.position = Vector3.Lerp(transform.position, player.transform.position, 3f * Time.deltaTime);

        GameObject camera = transform.GetChild(0).gameObject;

        if (_shakeTimer > 0f)
        {
            _shakeTimer -= Time.deltaTime;
            float shakeX = Random.Range(-0.1f, 0.1f) * _shakeTimer / _shakeDuration;
            float shakeY = Random.Range(-0.1f, 0.1f) * _shakeTimer / _shakeDuration;
            camera.transform.localPosition = CAMERA_OFFSET + new Vector3(shakeX, 0, shakeY);
        }
        else
        {
            camera.transform.localPosition = CAMERA_OFFSET;
        }
    }
}
