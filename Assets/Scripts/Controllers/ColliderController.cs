using UnityEngine;
using UnityEngine.InputSystem;

public class ColliderController : MonoBehaviour
{
    private int _enemyLayer;

    private float _attack;

    private const float _attackActivationThres = 0.4f;
    private const float _attackCooldown = 0.5f;

    private CameraController _camera;

    private GameObject player;

    void Awake()
    {
        _camera = GameObject.Find("CameraHolder").GetComponent<CameraController>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Start()
    {
        player = GameObject.Find("Player");
        GameManager.GetInstance().Input.Player.Attack.performed += HandleAttack;
    }

    void OnDestroy()
    {
        GameManager.GetInstance().Input.Player.Attack.performed -= HandleAttack;
    }

    void HandleAttack(InputAction.CallbackContext ctx)
    {
        _attack = _attackCooldown;
        GameManager.GetInstance().BroadcastPlayerAttack(transform.position);
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
