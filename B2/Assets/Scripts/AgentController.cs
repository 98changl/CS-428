using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    //private bool selected = false;
    private Vector2 smoothPos = Vector2.zero;
    private Vector2 velocity = Vector2.zero;

    Animator animator;
    NavMeshAgent agent;
    Direction direction;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        direction = GetComponent<Direction>();
        agent.updatePosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.destination = hit.point;
            }
        }
        Move();
    }

    void Move()
    {
        // set up position for ground plane
        Vector3 worldPos = agent.nextPosition - transform.position;
        Vector2 pos = new Vector2(Vector3.Dot(transform.right, worldPos), Vector3.Dot(transform.forward, worldPos));
        
        // filter the move
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothPos = Vector2.Lerp(smoothPos, pos, smooth);

        velocity = smoothPos / Time.deltaTime;

        bool moving = false;
        if (velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius)
        {
            moving = true;
        }

        // update animator
        animator.SetBool("isMoving", moving);
        animator.SetFloat("x", velocity.x);
        animator.SetFloat("y", velocity.y);

        if (direction)
        {
            direction.lookAtTarget = agent.steeringTarget + transform.forward;
        }

        if (worldPos.magnitude > agent.radius)
        {
            transform.position = agent.nextPosition - 0.9f * worldPos;
        }
    }

    void OnAnimatorMove()
    {
        Vector3 pos = animator.rootPosition;
        pos.y = agent.nextPosition.y;
        transform.position = pos;
    }

    /*
    // Detect if agent was clicked
    void OnMouseDown()
    {
        //If the agent is not selected, set the color to red, else set it to green.

        //If the agent is selected and was clicked again, deselect the agent.
        if (selected == true)
        {
            selected = false;
        }
        //If the agent is not selected and is clicked, select the agent.
        else
        {
            selected = true;
        }
    }
    */
}
