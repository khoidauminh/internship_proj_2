using UnityEngine;
using UnityEngine.AI;

public class SampleNagivation : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(end.position);   
    }
}
