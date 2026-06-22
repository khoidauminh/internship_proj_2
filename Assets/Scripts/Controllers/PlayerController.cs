using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Animator _animator;
    private CameraController _camera;
    private ColliderController _collider;
    private Rigidbody _rb;

    void Awake()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _collider = FindAnyObjectByType<ColliderController>();
        _camera = FindAnyObjectByType<CameraController>();
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _initialPosition = transform.position;
        GameManager.GetInstance().OnLevelUp += ResetPosition;
    }

    void OnDestroy()
    {
        GameManager.GetInstance().OnLevelUp -= ResetPosition;
    }

    public void ResetPosition(int _level)
    {
        transform.position = _initialPosition;
    }

    public void Update()
    {
        Vector2 inputDirection = GameManager.GetInstance().MoveDirection();
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
