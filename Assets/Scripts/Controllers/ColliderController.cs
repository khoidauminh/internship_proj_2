using UnityEngine;

public class ColliderController : MonoBehaviour
{
    
    private PlayerInput _input;

    private int _enemyLayer;

    private bool _toAttack;

    private float _attack;

    private const float _attackActivationThres = 0.4f;
    private const float _attackCooldown = 0.5f;

    private GameObject player;

    void Awake()
    {
        _input = new PlayerInput();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Start()
    {
        player = GameObject.Find("Player");

        _input.Player.Attack.performed += ctx => {
            _attack = _attackCooldown;
        };
    }

    void OnEnable()
    {
        _input.Enable();
    }

    void OnDisable()
    {
        _input.Disable();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == _enemyLayer && _attack >= _attackActivationThres)
        {
            _attack = 0;
            other.gameObject.GetComponent<BaseUnitController>().Die();
            GameObject.Find("CameraHolder").GetComponent<CameraController>().Shake();
        }
    }

    public void CustomUpdate()
    {
        transform.position = player.transform.position;

        if (_attack >= 0)
            _attack -= Time.deltaTime;
    }
}
