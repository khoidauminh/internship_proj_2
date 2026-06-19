using UnityEngine;
using UnityEngine.InputSystem;

public class ColliderController : MonoBehaviour
{

    private PlayerInput _input;

    private int _enemyLayer;

    private float _attack;

    private const float _attackActivationThres = 0.4f;
    private const float _attackCooldown = 0.5f;

    private CameraController _camera;

    private GameObject player;

    void Awake()
    {
        _input = new PlayerInput();
        _camera = GameObject.Find("CameraHolder").GetComponent<CameraController>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Start()
    {
        player = GameObject.Find("Player");
        _input.Player.Attack.performed += HandleAttack;
        GameManager.GetInstance().OnPause += HandlePause;
    }

    void OnDestroy()
    {
        _input.Player.Attack.performed -= HandleAttack;
        GameManager.GetInstance().OnPause -= HandlePause;
    }

    void HandlePause(bool p)
    {
        if (p) _input.Disable(); else _input.Enable();
    }

    void HandleAttack(InputAction.CallbackContext ctx)
    {
        _attack = _attackCooldown;
        GameManager.GetInstance().BroadcastPlayerAttack(transform.position);
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }

    public bool IsAttacking()
    {
        return _attack >= _attackActivationThres;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == _enemyLayer && IsAttacking())
        {
            _attack = 0;
            GameManager.GetInstance().BroadCastPlayerKill(transform.position, other.gameObject.transform.position);
            other.gameObject.GetComponent<BaseUnitController>().Die();

            _camera.Shake();
        }
    }

    public void CustomUpdate()
    {
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;

        if (_attack >= 0)
            _attack -= Time.deltaTime;
    }
}
