using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public float radius;
    public float mass;
    public float perceptionRadius;
    public float radiusExpander = -30f;

    //-----------------------------------------------------------------------------
    /* The remove agent boolean is responsible for making sure that the agent
     * is not removed after it reaches its destination. When testing the spiral
     * functionality set this boolean to false so the agent is not removed once
     * it hits its destination.
     */
    private bool removeAgent = false;
    //-----------------------------------------------------------------------------

    private List<Vector3> path;
    private NavMeshAgent nma;
    private Rigidbody rb;

    private HashSet<GameObject> perceivedNeighbors = new HashSet<GameObject>();

    //Leader/Follower Variables
    private List<GameObject> agents;
    private GameObject leader;

    void Start()
    {
        path = new List<Vector3>();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        nma.radius = radius;
        rb.mass = mass;
        GetComponent<SphereCollider>().radius = perceptionRadius / 2;

        //Leader/Follower
        // agents = new List<GameObject>(AgentManager.agentsObjs.Keys);
        // agents[0].name = "Leader";
        // leader = GameObject.Find("Leader");
    }

    private void Update()
    {
        if (path.Count > 1 && Vector3.Distance(transform.position, path[0]) < 1.1f)
        {
            path.RemoveAt(0);
        } else if (path.Count == 1 && Vector3.Distance(transform.position, path[0]) < 2f)
        {
            path.RemoveAt(0);

            if (path.Count == 0 && removeAgent == true)
            {
                gameObject.SetActive(false);
                AgentManager.RemoveAgent(gameObject);
            }
        }

        #region Visualization

        if (false)
        {
            if (path.Count > 0)
            {
                Debug.DrawLine(transform.position, path[0], Color.green);
            }
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.yellow);
            }
        }

        if (false)
        {
            foreach (var neighbor in perceivedNeighbors)
            {
                Debug.DrawLine(transform.position, neighbor.transform.position, Color.yellow);
            }
        }

        #endregion
    }

    #region Public Functions

    public void ComputePath(Vector3 destination)
    {
        nma.enabled = true;
        var nmPath = new NavMeshPath();
        nma.CalculatePath(destination, nmPath);
        path = nmPath.corners.Skip(1).ToList();
        //path = new List<Vector3>() { destination };
        //nma.SetDestination(destination);
        nma.enabled = false;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    #endregion

    #region Incomplete Functions

    private Vector3 ComputeForce()
    {
        var force = Vector3.zero;

        force = CalculateGoalForce() + CalculateAgentForce() + CalculateWallForce();
        
        //This code block below is for the pursue and evade forces
        /*
        force = (Pursue() * 1.5f) + (Evade().normalized * 0.6f) + CalculateAgentForce() + CalculateWallForce();
        if(rb.velocity.magnitude > 4f)
        {
            rb.velocity = rb.velocity.normalized * 4f;
        }
        */

        // to test spiral force behavior, uncomment the bottom code to override goal force
        //force = SpiralForce() + CalculateAgentForce() + CalculateWallForce();

        // to test crowd following, uncomment the bottom code to override goal force
        //force += CalculateCrowdFollow();

        // to test flock behavior, uncomment the bottom code to override goal force
        //force = CalculateFlock() + CalculateAgentForce() + CalculateWallForce();

        //Leader/Follower
        // force -= CalculateGoalForce();
        // if((rb.name).Equals("Leader"))
        // {
        //     force +=CalculateGoalForce();
        // } else
        // {
        //     force += calculateFollowerForce();
        // }

        if (force != Vector3.zero)
        {
            return Mathf.Min(force.magnitude, Parameters.maxSpeed) * force.normalized;
        } else
        {
            return Vector3.zero;
        }
    }
    
    private Vector3 CalculateGoalForce()
    {
        if (path.Count == 0)
        {
            return Vector3.zero;
        }

        var velocity = (path[0] - transform.position).normalized * Mathf.Min((path[0] - transform.position).magnitude, Parameters.maxSpeed);
        return mass * (velocity - rb.velocity) / Parameters.T;

        /*
        var val = temp.normalized * Mathf.Min(temp.magnitude, Parameters.maxSpeed);

        Debug.Log(mass);
        Vector3 distanceToDestination = (rb.position - nma.destination).normalized;
        var val2 = distanceToDestination * Mathf.Min(distanceToDestination.magnitude, Parameters.maxSpeed);
        //Debug.Log(mass * (((Parameters.maxSpeed * val) - rb.velocity) / Parameters.T));
        return mass * (((Parameters.maxSpeed * val) - rb.velocity) / Parameters.T);
        */
    }

    private Vector3 CalculateAgentForce()
    {
        var agentForce = Vector3.zero;

        foreach (var n in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(n))
            {
                continue;
            }

            var neighbor = AgentManager.agentsObjs[n];

            var overlap = (radius + neighbor.radius) - Vector3.Distance(transform.position, neighbor.transform.position); // rij - dij
            var psychological = Parameters.A * Mathf.Exp(overlap / Parameters.B);

            var g = 0;
            if (overlap > 0f)
            {
                g = 1;
                //Debug.Log("overlap");
            }
            var non_penetration = Parameters.k * g * overlap;

            var dir = (transform.position - neighbor.transform.position).normalized; // nij
            agentForce += (psychological + non_penetration) * dir;

            var tangent = Vector3.Cross(Vector3.up, dir);
            var tangental_velocity = Vector3.Dot(rb.velocity - neighbor.GetVelocity(), tangent);
            var friction = Parameters.Kappa * g * overlap * tangental_velocity * tangent;

            agentForce += friction; // force of sliding friction
        }

        return agentForce;
    }

    private Vector3 CalculateWallForce()
    {
        var wallForce = Vector3.zero;

        foreach (var n in perceivedNeighbors)
        {
            if (!WallManager.IsWall(n))
            {
                continue;
            }

            GameObject neighbor = n;

            var overlap = (radius + (neighbor.transform.localScale.x / 2)) - Vector3.Distance(transform.position, neighbor.transform.position);
            var psychological = Parameters.WALL_A * Mathf.Exp(overlap / Parameters.WALL_B);

            var g = 0;
            if (overlap > 0f)
                g = 1;
            var non_penetration = Parameters.WALL_k * g * overlap;

            var dir = (transform.position - neighbor.transform.position).normalized; // nij
            wallForce += (psychological + non_penetration) * dir;

            var tangent = Vector3.Cross(Vector3.up, dir);
            var tangental_velocity = Vector3.Dot(rb.velocity, tangent);
            var friction_force = Parameters.WALL_Kappa * g * overlap * tangental_velocity * tangent;

            wallForce -= friction_force;
        }

        return wallForce;
    }

    private Vector3 SpiralForce()
    {
        if (path.Count == 0)
        {
            return Vector3.zero;
        }

        //var temp = path[0] - transform.position + new Vector3(sinCurve, 0, cosCurve);
        var temp = path[0] - transform.position;
        temp = Quaternion.AngleAxis(radiusExpander, Vector3.up) * temp;

        if (radiusExpander > -90f)
        {
            radiusExpander -= 0.13f;
        }
        /*
        if (Spiralradius < 10)
        {
            Spiralradius += 0.008f;
        }
        adder += 0.3f * Time.deltaTime;
        */
        var desiredVel = temp.normalized * Mathf.Min(temp.magnitude, 4);
        var actualVelocity = rb.velocity;

        //Debug.DrawLine(transform.position, temp, Color.yellow);
        //Debug.DrawLine(transform.position, temp, Color.green, 0.5f);

        return mass * (desiredVel - actualVelocity) / Parameters.T;
    }

    private Vector3 calculateFollowerForce()
    {
        var leaderVelocity =  leader.GetComponent<Rigidbody>().velocity;
        var force = Vector3.zero;

        //Detect if agent is in front of leader
        var temp = (transform.position - leader.transform.position).normalized;
        var dot = Vector3.Dot(temp, leader.transform.forward);

        //Agent is in front of leader
        if(dot > 0)
        {
            var magnitude = Mathf.Exp(-Vector3.Angle(leaderVelocity, rb.velocity));
            //Debug.Log("Angular Derivation: "+Vector3.Angle(leaderVelocity, rb.velocity));
            //Debug.Log("Leader Velocity: "+leaderVelocity);
            //Debug.Log("rb.velocity: "+rb.velocity);
            //Debug.Log("Magnitude: "+ magnitude);
            var tangent = Vector3.Cross(Vector3.up, temp);
            //Debug.Log("tangent: "+tangent);
            var tangental_velocity = Vector3.Dot(rb.velocity, tangent);
            //Debug.Log("tangental_velocity: "+tangental_velocity);

            force = tangental_velocity * ((float)magnitude) * tangent;
            force.y = 0;
        }
        return force;
    }

    public void ApplyForce()
    {
        var force = ComputeForce();
        force.y = 0;

        //force += CalculateCrowdFollow();
        rb.AddForce(force / mass, ForceMode.Acceleration);
    }

    public void OnTriggerEnter(Collider other)
    {
        perceivedNeighbors.Add(other.gameObject);
    }
    
    public void OnTriggerExit(Collider other)
    {
        perceivedNeighbors.Remove(other.gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        perceivedNeighbors.Add(collision.gameObject);
    }

    public void OnCollisionExit(Collision collision)
    {
        perceivedNeighbors.Remove(collision.gameObject);
    }

    private Vector3 CalculateCrowdFollow()
    {
        var weight = 10.0f;
        var averageForce = Vector3.zero;

        if (path.Count == 0)
        {
            return averageForce;
        }

        var count = 0;
        foreach (var n in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(n))
            {
                continue;
            }

            var neighbor = AgentManager.agentsObjs[n];
            averageForce += neighbor.GetVelocity();
            count += 1;
        }

        if (count == 0)
            return averageForce;

        averageForce /= count;
        var desiredVel = averageForce.normalized * Mathf.Min(averageForce.magnitude, weight);
        return mass * (desiredVel - rb.velocity) / Parameters.T;
    }

    private Vector3 CalculateFlock()
    {
        var alignmentWeight = 2f;
        var cohesionWeight = 2f;
        var seperationWeight = 2f;
        var force = CalculateAlignment() * alignmentWeight + CalculateCohesion() * cohesionWeight + CalculateSeperation() * seperationWeight;

        var velocity = Mathf.Min(force.magnitude, 5) * force.normalized;
        return mass * (velocity - rb.velocity) / Parameters.T;
    }

    private Vector3 CalculateAlignment()
    {
        var alignmentForce = Vector3.zero;
        var count = 0;

        foreach (var n in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(n))
            {
                continue;
            }

            var neighbor = AgentManager.agentsObjs[n];
            alignmentForce += neighbor.GetVelocity();
            count += 1;
        }

        if (count == 0)
            return Vector3.forward;

        alignmentForce /= count;
        return alignmentForce;
    }

    private Vector3 CalculateCohesion()
    {
        var cohesionForce = Vector3.zero;
        var count = 0;

        foreach (var n in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(n))
            {
                continue;
            }

            var neighbor = AgentManager.agentsObjs[n];
            cohesionForce += neighbor.transform.position;
            count += 1;
        }

        if (count == 0)
            return cohesionForce;

        cohesionForce /= count;
        cohesionForce -= transform.position;
        return cohesionForce;
    }

    private Vector3 CalculateSeperation()
    {
        var seperationForce = Vector3.zero;
        var avoidanceRadius = perceptionRadius / 2;
        var count = 0;

        foreach (var n in perceivedNeighbors)
        {
            if (!AgentManager.IsAgent(n))
            {
                continue;
            }

            var neighbor = AgentManager.agentsObjs[n];

            var magnitude = (neighbor.transform.position - transform.position).sqrMagnitude;
            if (magnitude < avoidanceRadius)
            {
                count += 1;
                seperationForce += (transform.position - neighbor.transform.position);
            }
        }

        if (count == 0)
            return seperationForce;

        seperationForce /= count;
        return seperationForce;
    }

    public Vector3 Seek(Vector3 agent_target)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 force = Vector3.zero;
        Vector3 desiredVel;
        Vector3 toTarget = agent_target - transform.position;
        toTarget.Normalize();
        desiredVel = toTarget * Parameters.maxSpeed;
        // force = desiredVel - rb.velocity;
        force = desiredVel - (velocity * Time.deltaTime);
        return force.normalized;
    }

    public Vector3 Pursue()
    {
        var agentScript = new List<Agent>(AgentManager.agentsObjs.Values);
        var agents = new List<GameObject>(AgentManager.agentsObjs.Keys);
        Vector3 toTarget;
        Vector3 pursueTargetPos = Vector3.zero;
        float lookAhead;
        var cnt = 0;
        foreach (var n in agents)
        {
            if (cnt % 2 == 0)
            {
                agents[cnt].name = "Evader" + cnt;
                var evader = GameObject.Find("Evader" + cnt);
                //var target = agents[0];
                toTarget = evader.transform.position - transform.position;
                lookAhead = toTarget.magnitude / Parameters.maxSpeed;
                pursueTargetPos = evader.transform.position + (evader.GetComponent<Agent>().GetVelocity() * lookAhead);
                //Vector3 pursueTargetPos = target.transform.position + (target.velocity * lookAhead);
                //return Seek(pursueTargetPos);
            }
            cnt++;
        }
        return Seek(pursueTargetPos);
    }

    public Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVel;
        Vector3 force = Vector3.zero;
        desiredVel = transform.position - targetPos;
        desiredVel.Normalize();
        desiredVel *= Parameters.maxSpeed;
        force = desiredVel - rb.velocity;

        return force;
    }
    public Vector3 Evade()
    {
        var agentScript = new List<Agent>(AgentManager.agentsObjs.Values);
        var agents = new List<GameObject>(AgentManager.agentsObjs.Keys);
        Vector3 displacement;
        Vector3 targetVel;
        float distance;
        float updatesNeeded;
        Vector3 targetFuturePosition = Vector3.zero;
        var cnt = 0;
        foreach (var n in agents)
        {
            Debug.Log(cnt);
            if (cnt % 2 != 0)
            {
                agents[cnt].name = "Pursuader" + cnt;
                var pursuader = GameObject.Find("Pursuader" + cnt);
                displacement = pursuader.transform.position - transform.position;
                distance = displacement.magnitude;
                updatesNeeded = distance / Parameters.maxSpeed;
                targetVel = pursuader.GetComponent<Agent>().GetVelocity();
                targetVel *= updatesNeeded;
                targetFuturePosition = new Vector3(pursuader.transform.position.x,
                                                            pursuader.transform.position.y,
                                                            pursuader.transform.position.z);
                targetFuturePosition += targetVel;
                //return Flee(targetFuturePosition);
            }
            cnt++;
        }
        return Flee(targetFuturePosition);
    }

    #endregion
}
