using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;
    public float movSpeed = 6.0f;
    public float rotSpeed = 1.0f;
    public int minDistance = 5;
    public int safeDistance = 60;

    public enum AIstates { Idle, Seek, Flee, Arrive, Pursuit, Evade}

    public AIstates currentState;

    // Start is called before the first frame update
    void Start()
    {
        //currentState = AIstates.Idle;
        //currentState = AIstates.Seek;
        // currentState = AIstates.Flee;
        //currentState = AIstates.Arrive;
        //currentState = AIstates.Pursuit;
        currentState = AIstates.Evade;
        agent = this.GetComponent<NavMeshAgent>();
        
    }

    //private void Update()
    //{
    //    Seek();
    //}

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AIstates.Idle:
                Debug.Log("Idle");
                break;
            case AIstates.Seek:
                Seek();
                Debug.Log("Seek");
                break;
            case AIstates.Flee:
                Flee();
                Debug.Log("Flee");
                break;
            case AIstates.Arrive:
                Arrive();
                Debug.Log("Arrive");
                break;
            case AIstates.Pursuit:
                Pursuit();
                Debug.Log("Pursuit");
                break;
            case AIstates.Evade:
                Evade();
                Debug.Log("Evade");
                break;
        }
    }

    //void Seek()
    //{
    //    Vector3 targetVector = target.transform.position;
    //    agent.SetDestination(targetVector);

    //}

    void Seek()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

        if (direction.magnitude > minDistance)
        {

            Vector3 moveVector = direction.normalized * movSpeed * Time.deltaTime;

            transform.position += moveVector;

        }
    }

    void Flee()
    {
        Vector3 direction = transform.position - target.position;
        direction.y = 0;

        if (direction.magnitude < safeDistance)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);
            Vector3 moveVector = direction.normalized * movSpeed * Time.deltaTime;
            transform.position += moveVector;
        }
    }

    void Arrive()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        float decelerationFactor  = distance / 5;

        float speed = movSpeed * decelerationFactor;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

        Vector3 moveVector = direction.normalized * Time.deltaTime * speed;
        transform.position += moveVector;
    }

    void Pursuit()
    {
        int iterationAhead = 30;

        var targetSpeed = target.gameObject.GetComponent<MovementController>().instantVelocity;

        Vector3 targetFuturePosition = target.transform.position + (targetSpeed * iterationAhead);

        Vector3 direction = targetFuturePosition - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

        if (direction.magnitude > minDistance)
        {

            Vector3 moveVector = direction.normalized * movSpeed * Time.deltaTime;

            transform.position += moveVector;

        }
    }

    void Evade()
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

}
