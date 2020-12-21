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

    public enum AIstates {Pursuit, Evade1, Evade2}

    public AIstates currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = AIstates.Evade2;
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
            case AIstates.Evade1:
                Evade1();
                Debug.Log("Evade1");
                break;
            case AIstates.Evade2:
                Evade2();
                Debug.Log("Evade2");
                break;
        }
    }

    void Pursuit()
    {
        Vector3 targetVector = target.transform.position;
        agent.SetDestination(targetVector);

    }

    void Evade1()
    {
        int iterationAhead = 15;

        var targetSpeed = target.gameObject.GetComponent<MovementController>().instantVelocity;

        Vector3 targetFuturePosition = target.position + (targetSpeed * iterationAhead);

        Vector3 direction = transform.position - targetFuturePosition;
        direction.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

        if (direction.magnitude < safeDistance)
        {

            Vector3 moveVector = direction.normalized * movSpeed * Time.deltaTime;

            transform.position += moveVector;

        }
    }

    public void Evade2()
    {

        // store the starting transform
        startTransform = transform;

        //temporarily point the object to look away from the player
        transform.rotation = Quaternion.LookRotation(transform.position - target.position);

        //Then we'll get the position on that rotation that's multiplyBy down the path (you could set a Random.range
        // for this if you want variable results) and store it in a new Vector3 called runTo
        Vector3 runTo = transform.position + transform.forward * multiplyBy;
        //Debug.Log("runTo = " + runTo);

        //So now we've got a Vector3 to run to and we can transfer that to a location on the NavMesh with samplePosition.

        NavMeshHit hit;    // stores the output in a variable called hit

        // 5 is the distance to check, assumes you use default for the NavMesh Layer name
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetNavMeshLayerFromName("Default"));
        //Debug.Log("hit = " + hit + " hit.position = " + hit.position);

        // reset the transform back to our start transform
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        // And get it to head towards the found NavMesh position
        agent.SetDestination(hit.position);
    }
}
