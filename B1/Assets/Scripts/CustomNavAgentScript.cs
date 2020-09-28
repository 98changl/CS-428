using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomNavAgentScript : MonoBehaviour
{
    // agent selection tools
    private bool selected = false;
    public Material agentDeselected;
    public Material agentSelected;
    public Renderer agentColor;

    // game objects for calculating agent movement
    public float speed;
    public GameObject self;
    public GameObject GameManager;

    private List<Node> Path;
    private Graph graph;
    private Navigation navi;
    private Vector3 TargetPosition;

    void Start()
    {
        graph = GameManager.GetComponent<Graph>();
        navi = GameManager.GetComponent<Navigation>();
    }

    void Update()
    {
        // Only move agents that are selected when right clicking.
        if (Input.GetMouseButtonDown(1) && selected == true)
        {
            RaycastHit hit;
            Debug.Log("Order Given");
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                TargetPosition = hit.point;

                Debug.Log("Start Pos: " + self.transform.position);
                Debug.Log("End Pos: " + TargetPosition);

                Path = navi.FindPath(self.transform.position, TargetPosition);
                if (Path == null)
                {
                    Debug.Log("Path failed");
                }
                else
                {
                    Traverse();
                }
            }
        }
    }

    // Detect if agent was clicked
    void OnMouseDown()
    {
        //If the agent is not selected, set the color to red, else set it to green.

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

    void Traverse()
    {
        Debug.Log("Traverse Started");
        Vector3 previous = transform.position;
        Node goTo;
        float startMove = 0.0f;

        while (Path.Count > 0)
        {
            goTo = Path[0];
            StartCoroutine(move(goTo.position, startMove));
            startMove += 2.0f;
            Path.Remove(goTo);
        }
    }

    IEnumerator move(Vector3 destination, float startMove)
    {
        yield return new WaitForSeconds(startMove);
        Vector3 start = transform.position;

        //transform.position = Vector3.MoveTowards(transform.position, destination, speed);
        float t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / 1.0f;
            transform.position = Vector3.Lerp(start, destination, t);
        }
        
        Debug.Log("Travelled to: " + destination);
    }

    //Unselect agent and set color back to red after exiting the game.
    void OnApplicationQuit()
    {
        selected = false;
        agentColor.material = agentDeselected;
    }
}
