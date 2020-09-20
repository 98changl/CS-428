using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{

    NavMeshAgent agent;
    private bool selected = false;
    public Material agentColor;


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

        Debug.Log(selected);

    }

    //Detect if agent was clicked
    void OnMouseDown()
    {
        //If the agent is not selected, set the color to red, else set it to red.

        //If the agent is selected and was clicked again, deselect the agent.
        if(selected == true)
        {
            selected = false;
            agentColor.color = new Color(255, 0, 0, 255);
        }
        //If the agent is not selected and is clicked, select the agent.
        else
        {
            selected = true;
            agentColor.color = Color.green;
        }

    }

    //Unselect agent and set color back to red after exiting the game.
    void OnApplicationQuit()
    {
        selected = false;
        agentColor.color = new Color(255, 0, 0, 255);
    }

}
