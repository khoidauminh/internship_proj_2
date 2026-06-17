using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    private  CameraController _camera;
    private ColliderController _collider;

    void Awake()
    {
        _input = new PlayerInput();
        _collider = FindAnyObjectByType<ColliderController>();
        _camera = FindAnyObjectByType<CameraController>();
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }

    public void CustomUpdate()
    {
        Vector2 inputDirection = _input.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        transform.Translate(moveDirection * Time.deltaTime * 5f, Space.World);
        _collider.CustomUpdate();
        _camera.CustomUpdate();
    }
}
