using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_controller : MonoBehaviour
{
    public Transform target;
    private Transform startTransform;
    public NavMeshAgent agent;
    public float movSpeed = 6.0f;
    public float rotSpeed = 1.0f;
    public int minDistance = 5;
    public int safeDistance = 60;
    public float multiplyBy = 1.1f;

    public enum AIstates {Pursuit, Evade}

    public AIstates currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = AIstates.Evade;
        agent = this.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("player").transform;

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AIstates.Pursuit:
                Pursuit();
                Debug.Log("Pursuit");
                break;
            case AIstates.Evade:
                Evade();
                Debug.Log("Evade1");
                break;
        }
    }

    void Pursuit()
    {
        Vector3 targetVector = target.transform.position;
        agent.SetDestination(targetVector);

    }

    public void Evade()
    {
        startTransform = transform;
        transform.rotation = Quaternion.LookRotation(transform.position - target.position);
        Vector3 runTo = transform.position + transform.forward * multiplyBy;
        NavMeshHit hit;    
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetNavMeshLayerFromName("Default"));

        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        agent.SetDestination(hit.position);
    }
}
