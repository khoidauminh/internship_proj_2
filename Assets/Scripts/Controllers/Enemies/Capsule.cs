using UnityEngine;
using UnityEngine.AI;

public class Capsule : BaseEnemy
{
    public NavMeshAgent agent;

    private void Start()
    {
        agent.speed = _currentSpeed;
    }

    public override void CustomUpdate()
    {
        agent.SetDestination(GameObject.Find("Player").transform.position);

        //transform.Translate(Random.Range(-0.05f, 0.05f), 0, Random.Range(-0.05f, 0.05f));

        //GameObject player = GameObject.Find("Player");

        //Vector3 offset = transform.position - player.transform.position;
        //offset.y = 0;

        //if (offset.magnitude < 4f)
        //{
        //    Move(offset.normalized);
        //}

        //if (transform.position.magnitude >= 10f)
        //{
        //    Vector3 dir = -transform.position.normalized;

        //    dir.y = 0;

        //    Move(dir);
        //}
    }
}
