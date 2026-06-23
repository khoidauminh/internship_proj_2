using UnityEngine;

public class Cylinder : BaseEnemy
{
    private Vector3 _targetPosition;
    private Vector3 _currentPosition;

    private float _reTargetTimer;

    public override void CustomUpdate()
    {
        transform.rotation = Quaternion.identity;

        _reTargetTimer -= Time.deltaTime;

        if (_reTargetTimer <= 0)
        {
            _reTargetTimer = Random.Range(2f, 5f);
            _currentPosition = transform.position;
            _targetPosition += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            _targetPosition = Vector3.ClampMagnitude(_targetPosition, 12f);
        }

        Vector3 diff = _targetPosition - transform.position;
        if (diff.magnitude > 0.1f)
        {
            Vector3 direction = diff.normalized;
            direction.y = 0;
            Move(direction);
        }

        base.CustomUpdate();
    }
}
