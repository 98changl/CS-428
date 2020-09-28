using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // x, y, z, coordinates in graph
    public int graphX;
    public int graphY;
    public int graphZ;
    public int distanceToNextNode;
    public int distanceFromEnd;

    public bool obstacle; // node is impassable
    public Vector3 position; // coordinates of the node on the map
    public Node parent;

    public Node (int x, int y, int z, bool obstacle, Vector3 position)
    {
        this.graphX = x;
        this.graphY = y;
        this.graphZ = z;
        this.obstacle = obstacle;
        this.position = position;
    }

    public int getTotalDistance()
    {
        return distanceToNextNode + distanceFromEnd;
    }
}
