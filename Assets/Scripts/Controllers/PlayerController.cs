using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;

    void Awake()
    {
        _input = new PlayerInput();
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }

    void Update()
    {
        Vector2 inputDirection = _input.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        transform.Translate(moveDirection * Time.deltaTime * 5f, Space.World);
    }
}
