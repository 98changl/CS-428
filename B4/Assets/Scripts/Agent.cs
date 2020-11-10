﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public float radius;
    public float mass;
    public float perceptionRadius;

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

    void Start()
    {
        path = new List<Vector3>();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        gameObject.transform.localScale = new Vector3(2 * radius, 1, 2 * radius);
        nma.radius = radius;
        rb.mass = mass;
        GetComponent<SphereCollider>().radius = perceptionRadius / 2;
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

        if (force != Vector3.zero)
        {
            return force.normalized * Mathf.Min(force.magnitude, Parameters.maxSpeed);
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

        var temp = path[0] - transform.position;
        var desiredVel = temp.normalized * Mathf.Min(temp.magnitude, Parameters.maxSpeed);
        var actualVelocity = rb.velocity;
        return mass * (desiredVel - actualVelocity) / Parameters.T;

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
                g = 1;
            var non_penetration =  (g * overlap) / Parameters.k;

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
            var non_penetration = (g * overlap) / Parameters.WALL_k;

            var dir = (transform.position - neighbor.transform.position).normalized; // nij
            wallForce += (psychological + non_penetration) * dir;

            var tangent = Vector3.Cross(Vector3.up, dir);
            var tangental_velocity = Vector3.Dot(rb.velocity, tangent);
            var friction_force = Parameters.WALL_Kappa * g * overlap * tangental_velocity * tangent;

            wallForce -= friction_force;
        }

        return wallForce;
    }

    public void ApplyForce()
    {
        var force = ComputeForce();
        force.y = 0;

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

    #endregion
}
