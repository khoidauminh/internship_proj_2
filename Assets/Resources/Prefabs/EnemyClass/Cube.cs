using UnityEngine;

public class Cube : BaseUnitController
{
    private Quaternion direction = Quaternion.Euler(0, 0f, 0);
    private float turnDelay = 1f;
    private float turnTimer;

    public override void CustomUpdate()
    {
        turnTimer -= Time.deltaTime;
        if (turnTimer <= 0)
        {
            turnTimer = turnDelay;

            Quaternion randomDirection = Quaternion.Euler(0, Random.Range(100f, 260f), 0);

            if (transform.position.magnitude > 10f)
            {
                Vector3 toCenter = -transform.position.normalized;
                direction = Quaternion.LookRotation(toCenter);
            } else
            {
                direction *= randomDirection;
            }
        }

        Move(direction * Vector3.forward);

        base.CustomUpdate();
    }
}
