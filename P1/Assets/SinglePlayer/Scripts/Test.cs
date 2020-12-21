using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField]
    Transform Player;
    public int multiplier = 1; // or more
    public float range = 30;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
     
    }

    private void Update()
    {
        Vector3 runTo = transform.position + ((transform.position - Player.position) * multiplier);
        float distance = Vector3.Distance(transform.position, Player.position);
        if (distance < range) agent.SetDestination(runTo);

    }
}
