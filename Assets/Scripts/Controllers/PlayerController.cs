using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    private Animator _animator;
    private CameraController _camera;
    private ColliderController _collider;
    private Rigidbody _rb;

    void Awake()
    {
        _input = new PlayerInput();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _collider = FindAnyObjectByType<ColliderController>();
        _camera = FindAnyObjectByType<CameraController>();
        _rb = GetComponent<Rigidbody>();
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

        if (moveDirection.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            _rb.AddForce(moveDirection * 10f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        }

        _collider.CustomUpdate();
        _camera.CustomUpdate();
        _animator.SetBool("attack", _collider.IsAttacking());
    }
}
