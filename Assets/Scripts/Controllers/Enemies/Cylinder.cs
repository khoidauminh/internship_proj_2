using UnityEngine;
using UnityEngine.AI;

public class Cylinder : BaseEnemy
{
    private float retargetTimer;
    public NavMeshAgent agent;

    void Start()
    {
        agent.speed = _currentSpeed;
    }

    public override void CustomUpdate()
    {
        retargetTimer -= Time.deltaTime;

        if (retargetTimer <= 0)
        {
            retargetTimer = Random.Range(2f, 5f);
            // _targetPosition += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            Transform region = GameObject.Find("NavRegion").transform;
            float x = Random.Range(region.position.x - region.localScale.x / 2f, region.position.x + region.localScale.x / 2f);
            float z = Random.Range(region.position.z - region.localScale.z / 2f, region.position.z + region.localScale.z / 2f);

            agent.SetDestination(new Vector3(z, 0, z));
        }

        base.CustomUpdate();
    }
}
