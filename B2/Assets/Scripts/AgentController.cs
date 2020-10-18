using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    public Material agentDeselected;
    public Material agentSelected;
    public Renderer agentColor;

    private bool selected = false;
    private bool jumping = false;
    private float speed;
    private float jumpAnimationBlend = 0;
    private float previousClick = 0f;
    private float timeBetweenClicks = 0.2f;

    public Vector2 velocity = Vector2.zero;

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
        if (Input.GetMouseButtonDown(0) && selected == true)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.destination = hit.point;
            }

            float click = Time.time - previousClick;
            if (click <= timeBetweenClicks) // multiple clicks
            {
                Debug.Log("Run");
                speed = 2f;
            }
            else
            {
                Debug.Log("Walk");
                speed = 0.75f;
            }
            previousClick = Time.time;
        }

        Move();

        // stops crowds from constantly moving
        if (agent.velocity == Vector3.zero)
        {
            animator.SetBool("isMoving", false);
        }
    }

    void Move()
    {
        // set up position for ground plane
        Vector3 worldPos = agent.nextPosition - transform.position;
        Vector2 pos = new Vector2(Vector3.Dot(transform.right, worldPos), Vector3.Dot(transform.forward, worldPos));

        if (pos.x > 0)
        {
            velocity.x = 1;
        }
        else
        {
            velocity.x = -1;
        }

        if (pos.y > 0)
        {
            velocity.y = 1;
        }
        else
        {
            velocity.y = -1;
        }

        bool moving = false;
        if (velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius)
        {
            moving = true;
        }

        // update animator
        animator.SetBool("isMoving", moving);
        animator.SetFloat("x", velocity.x, .1f, Time.deltaTime);
        animator.SetFloat("y", velocity.y * speed, .1f, Time.deltaTime);

        // set object look direction
        if (direction)
        {
            direction.lookAtTarget = agent.steeringTarget + transform.forward;
        }

        // move object to agent
        if (worldPos.magnitude > agent.radius)
        {
            if (jumping)
            {
                //Debug.Log("Jumping");
                Vector3 Jump = new Vector3(0, 5, 0);
                Jump.Normalize();
                transform.position = (agent.nextPosition - 0.9f * worldPos) + Jump;
            }
            else
            {
                transform.position = agent.nextPosition - 0.9f * worldPos;
            }
        }

        Jump();

    }
    
    void Jump()
    {
        if (agent.isOnOffMeshLink)
        {
            Debug.Log("Jump");
            jumpAnimationBlend += 0.2f * Time.deltaTime;
            animator.SetBool("isAirborne", true);
            animator.SetFloat("Jump Blend", jumpAnimationBlend);
            jumping = true;
        }
        else
        {
            animator.SetBool("isAirborne", false);
            jumpAnimationBlend = 0;
            jumping = false;
        }
    }

    void OnAnimatorMove()
    {
        Vector3 pos = animator.rootPosition;
        pos.y = agent.nextPosition.y;
        transform.position = pos;
    }

    //Detect if agent was clicked
    void OnMouseOver()
    {
        // detect a right click when mouse is hovering over the collider
        if (Input.GetMouseButtonDown(1))
        {
            //If the agent is selected and was clicked again, deselect the agent.
            if (selected == true)
            {
                selected = false;
                agentColor.material = agentDeselected;
            }
            //If the agent is not selected and is clicked, select the agent.
            else
            {
                selected = true;
                agentColor.material = agentSelected;
            }
        }
        
    }

}
