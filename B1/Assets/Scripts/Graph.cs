using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Graph : MonoBehaviour
{
    // required inputs for graph
    public LayerMask Obstacle;
    public Vector3 world;
    public float radius;
    public float Distance;

    // graph variables
    Node[,,] graph;
    float diameter;
    int graphX; // max size of x
    int graphY; // max size of y
    int graphZ; // max size of z

    // mesh variables
    NavMeshTriangulation navmeshData;
    Mesh mesh;

    void Start()
    {
        diameter = radius * 2;
        graphX = Mathf.RoundToInt(world.x / diameter);
        graphY = Mathf.RoundToInt(world.y / diameter);
        graphZ = Mathf.RoundToInt(world.z / diameter);
        CreateGraph();
        Debug.Log("Graph created");
    }

    void CreateGraph()
    {
        graph = new Node[graphX, graphY, graphZ];
        navmeshData = NavMesh.CalculateTriangulation();
        mesh = new Mesh();

        // set up mesh data
        mesh.vertices = navmeshData.vertices;
        mesh.SetIndices(navmeshData.indices, MeshTopology.Triangles, 0);
        
        Vector3 bottomLeft = transform.position - Vector3.right * world.x / 2 - Vector3.up * world.y / 2 - Vector3.forward * world.z / 2;

        for (int z = 0; z < graphZ; z++)
        {
            for (int y = 0; y < graphY; y++)
            {
                for (int x = 0; x < graphX; x++)
                {
                    Vector3 nodePosition = new Vector3((x * diameter + radius), (y * diameter + radius), (z * diameter + radius));
                    nodePosition += bottomLeft;
                    bool impassable = true;
                    
                    if (IsInsideMesh(nodePosition))
                    {
                        impassable = false;
                    }
                    
                    if (Physics.CheckSphere(nodePosition, radius, Obstacle))
                    {
                        impassable = false;
                    }
                    
                    graph[x, y, z] = new Node(x, y, z, impassable, nodePosition);
                    //Debug.Log("Graph: " + x + "," + y + "," + z + " has position " + nodePosition);
                }
            }
        }
    }

    bool IsInsideMesh(Vector3 point)
    {
        Vector3[] v = mesh.vertices;
        int[] tri = mesh.triangles;
        int total = tri.Length / 3;

        for (int i = 0; i < total; i++)
        {
            Vector3 a = v[tri[i * 3]];
            Vector3 b = v[tri[i * 3 + 1]];
            Vector3 c = v[tri[i * 3 + 2]];

            Plane p = new Plane(a, b, c);
            if (p.GetSide(point))
            {
                return false;
            }
        }
        return true;
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(world.x, world.y, world.z));

        if (graph != null)
        {
            foreach (Node node in graph)
            {
                if (node.obstacle)
                {
                    Gizmos.color = Color.clear;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawCube(node.position, Vector3.one * (diameter - Distance));
            }
        }
    }
    */

    public Node NodeFromPosition(Vector3 pos)
    {
        float x, y, z;
        x = (float) Math.Round(pos.x * 2, MidpointRounding.ToEven) / 2;
        y = (float) Math.Round(pos.y * 2, MidpointRounding.ToEven) / 2;
        z = (float) Math.Round(pos.z * 2, MidpointRounding.ToEven) / 2;

        if (x % 1 == 0)
        {
            x += 0.5f;
        }
        if (y % 1 == 0)
        {
            y += 0.5f;
        }
        if (z % 1 == 0)
        {
            z += 0.5f;
        }
        // Debug.Log("Rounding: " + x + "," + y + "," + z);
        Vector3 rounded = new Vector3(x, y, z);
        Debug.Log("Rounded Vector: " + rounded);

        int ix, iy, iz;
        for (iz = 0; iz < graphZ; iz++)
        {
            for (iy = 0; iy < graphY; iy++)
            {
                for (ix = 0; ix < graphX; ix++)
                {
                    //Debug.Log("Checking: " + graph[ix, iy, iz].position);
                    if (graph[ix, iy, iz].position.x == rounded.x)
                    {
                        if (graph[ix, iy, iz].position.y == rounded.y)
                        {
                            if (graph[ix, iy, iz].position.z == rounded.z)
                            {
                                return graph[ix, iy, iz];
                            }
                        }
                    }
                }
            }
        }

        return graph[0, 0, 0];
    }
    
    public List<Node> GetNeighborNodes(Node current)
    {
        List<Node> neighbors = new List<Node>();
        int x, y, z;

        // east side
        x = current.graphX + 1;
        y = current.graphY;
        z = current.graphZ;
        if (x >= 0 && x < graphX)
        {
            if (y >= 0 && y < graphY)
            {
                if (z >= 0 && z < graphZ)
                {
                    neighbors.Add(graph[x, y, z]);
                }
            }
        }

        // west side
        x = current.graphX - 1;
        y = current.graphY;
        z = current.graphZ;
        if (x >= 0 && x < graphX)
        {
            if (y >= 0 && y < graphY)
            {
                if (z >= 0 && z < graphZ)
                {
                    neighbors.Add(graph[x, y, z]);
                }
            }
        }

        // north side
        x = current.graphX;
        y = current.graphY;
        z = current.graphZ + 1;
        if (x >= 0 && x < graphX)
        {
            if (y >= 0 && y < graphY)
            {
                if (z >= 0 && z < graphZ)
                {
                    neighbors.Add(graph[x, y, z]);
                }
            }
        }

        // south side
        x = current.graphX;
        y = current.graphY;
        z = current.graphZ - 1;
        if (x >= 0 && x < graphX)
        {
            if (y >= 0 && y < graphY)
            {
                if (z >= 0 && z < graphZ)
                {
                    neighbors.Add(graph[x, y, z]);
                }
            }
        }

        // up side
        x = current.graphX;
        y = current.graphY + 1;
        z = current.graphZ;
        if (x >= 0 && x < graphX)
        {
            if (y >= 0 && y < graphY)
            {
                if (z >= 0 && z < graphZ)
                {
                    neighbors.Add(graph[x, y, z]);
                }
            }
        }

        // down side
        x = current.graphX;
        y = current.graphY - 1;
        z = current.graphZ;
        if (x >= 0 && x < graphX)
        {
            if (y >= 0 && y < graphY)
            {
                if (z >= 0 && z < graphZ)
                {
                    neighbors.Add(graph[x, y, z]);
                }
            }
        }

        return neighbors;
    }

}
