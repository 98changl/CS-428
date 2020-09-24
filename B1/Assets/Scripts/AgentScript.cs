using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{

    NavMeshAgent agent;
    private bool selected = false;
    public Material agentDeselected;
    public Material agentSelected;
    public Renderer agentColor;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //Only move agents that are selected when right clicking.
        if (Input.GetMouseButtonDown(1) && selected == true)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.destination = hit.point;
            }
        }
    }

    //Detect if agent was clicked
    void OnMouseDown()
    {
        //If the agent is not selected, set the color to red, else set it to green.

        //If the agent is selected and was clicked again, deselect the agent.
        if(selected == true)
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

    //Unselect agent and set color back to red after exiting the game.
    void OnApplicationQuit()
    {
        selected = false;
        agentColor.material = agentDeselected;
    }

}
