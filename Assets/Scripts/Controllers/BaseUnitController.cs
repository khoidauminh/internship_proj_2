using UnityEngine;

public class BaseUnitController : MonoBehaviour
{
    private BaseUnitConfig.Stats _stats;
    public BaseUnitConfig.Stats Stats => _stats;

    protected int _currentHealth;
    protected int _currentDamage;
    protected float _currentSpeed;

    public void Initialize(BaseUnitConfig.Stats config)
    {
        _stats = config;
        _currentHealth = _stats.BaseHealth;
        _currentDamage = _stats.BaseDamage;
        _currentSpeed = _stats.BaseSpeed;
        transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
    }

    public virtual void CustomUpdate()
    {
        if (transform.position.y < -10f)
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
        UnitPool.GetInstance().ReleaseUnit(this);
    }
}
