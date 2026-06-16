using UnityEngine;

public class BaseUnitController : MonoBehaviour
{
    private BaseUnitConfig _config;
    public BaseUnitConfig Config => _config;

    protected int _currentHealth;
    protected int _currentDamage;
    protected float _currentSpeed;

    public void Initialize(BaseUnitConfig config)
    {
        _config = config;
        _currentHealth = _config.BaseHealth;
        _currentDamage = _config.BaseDamage;
        _currentSpeed = _config.BaseSpeed;
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
    }

    public virtual void CustomUpdate()
    {
        if (transform.position.y < -100f)
        {
            Die();
        }
    }

    protected void Move(Vector3 direction)
    {
        transform.Translate(direction * _currentSpeed * Time.deltaTime, Space.World);
    }

    public void Attack(BaseUnitController target)
    {
        if (target == null)
        {
            return;
        }

        target.TakeDamage(_currentDamage);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        UnitPool.Instance.ReleaseUnit(this);
    }
}
