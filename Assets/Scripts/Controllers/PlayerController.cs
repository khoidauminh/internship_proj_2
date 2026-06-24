using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Animator _animator;
    private CameraController _camera;
    private ColliderController _collider;
    private Rigidbody _rb;

    [SerializeField] private Renderer _renderer;

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

        transform.rotation = GameObject.Find("CameraHolder").transform.rotation;

        if (moveDirection.magnitude > 0.1f)
        {
            transform.Translate(moveDirection * Time.deltaTime * 5f);
        }
        
        if (_collider.HurtTimer > 0f)
        {
            _renderer.enabled = (_collider.HurtTimer % 0.1) > 0.05;
        }
        else
        {
            _renderer.enabled = true;
        }

        _collider.CustomUpdate();
        _camera.CustomUpdate();
        _animator.SetBool("attack", _collider.IsAttacking());
    }
}
