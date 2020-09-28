using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    Graph graph;

    void Start()
    {
        graph = GetComponent<Graph>();
    }

    public List<Node> FindPath(Vector3 Start, Vector3 Target)
    {
        Debug.Log("Finding Path");
        Node StartNode = new Node(0, 0, 0, false, Start);
        StartNode = graph.NodeFromPosition(Start);
        Debug.Log("Start Node: " + StartNode.position);

        Node TargetNode = new Node(0, 0, 0, false, Target);
        TargetNode = graph.NodeFromPosition(Target);
        Debug.Log("End Node: " + TargetNode.position);

        List<Node> NodeList = new List<Node>();
        HashSet<Node> Visited = new HashSet<Node>();

        NodeList.Add(StartNode);

        //Debug.Log("Beginning loop");
        while (NodeList.Count > 0)
        {
            Node current = NodeList[0];
            for (int i = 1; i < NodeList.Count; i++) // chooses the most optimal node to travel to from node list
            {
                if (NodeList[i].getTotalDistance() < current.getTotalDistance() || NodeList[i].getTotalDistance() == current.getTotalDistance() && NodeList[i].distanceFromEnd < current.distanceFromEnd)
                {
                    current = NodeList[i];
                }
            }

            NodeList.Remove(current);
            Visited.Add(current);

            // target node has been reached
            if (current == TargetNode)
            {
                return GetPath(StartNode, TargetNode);
            }

            // checks each neighbor of current node
            foreach (Node neighbor in graph.GetNeighborNodes(current))
            {
                if (!neighbor.obstacle || Visited.Contains(neighbor))
                {
                    continue;
                }

                int MoveCost = current.distanceToNextNode + ManhattenDistance(current, neighbor);

                if (MoveCost < neighbor.distanceToNextNode || !NodeList.Contains(neighbor))
                {
                    neighbor.distanceToNextNode = MoveCost;
                    neighbor.distanceFromEnd = ManhattenDistance(neighbor, TargetNode);
                    neighbor.parent = current;

                    if (!NodeList.Contains(neighbor))
                    {
                        NodeList.Add(neighbor); // adds valid neighbor to list of nodes agent can travel to
                    }
                }
            }
        }

        return null;
    }

    List<Node> GetPath(Node StartNode, Node TargetNode)
    {
        List<Node> Path = new List<Node>();
        Node current = TargetNode;

        while (current != StartNode)
        {
            //Debug.Log("Path: " + current.position);
            Path.Add(current);
            current = current.parent;
        }

        Path.Reverse();
        Debug.Log("Path created");

        return Path;
    }

    int ManhattenDistance(Node a, Node b)
    {
        int x = Mathf.Abs(a.graphX - b.graphX);
        int y = Mathf.Abs(a.graphY - b.graphY);
        int z = Mathf.Abs(a.graphZ - b.graphZ);

        return x + y + z;
    }

}
