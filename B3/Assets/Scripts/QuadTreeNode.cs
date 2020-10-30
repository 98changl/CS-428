using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeNode
{
    // Bounding Box (AABB)
    Vector3 topLeftBound;       // max
    Vector3 bottomRightBound;   // min

    // list of Prisms in the quadrant
    public List<Prism> prisms;

    // QuadTree child nodes
    QuadTreeNode topLeft;
    QuadTreeNode topRight;
    QuadTreeNode bottomLeft;
    QuadTreeNode bottomRight;

    public QuadTreeNode(Vector3 topLeftBound, Vector3 bottomRightBound)
    {
        this.topLeftBound = topLeftBound;
        this.bottomRightBound = bottomRightBound;

        this.prisms = new List<Prism>();
        this.topLeft = null;
        this.topRight = null;
        this.bottomLeft = null;
        this.bottomRight = null;
    }

    // creates the entire quadtree for the field
    public void initializeTree(int depth, int totalDepth)
    {
        if (depth == totalDepth)
        {
            //Debug.Log("Depth reached");
            return;
        }

        Vector3 tempTopLeft = Vector3.zero;
        Vector3 tempBottomRight = Vector3.zero;

        // in top left quadrant
        tempTopLeft = this.topLeftBound;
        tempBottomRight.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempBottomRight.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        topLeft = new QuadTreeNode(tempTopLeft, tempBottomRight);
        topLeft.initializeTree(depth + 1, totalDepth);

        // in bottom right quadrant
        tempTopLeft.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempTopLeft.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        tempBottomRight = this.bottomRightBound;
        bottomRight = new QuadTreeNode(tempTopLeft, tempBottomRight);
        bottomRight.initializeTree(depth + 1, totalDepth);

        // in bottom left quadrant
        tempTopLeft.x = topLeftBound.x;
        tempTopLeft.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        tempBottomRight.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempBottomRight.z = this.bottomRightBound.z;
        bottomLeft = new QuadTreeNode(tempTopLeft, tempBottomRight);
        bottomLeft.initializeTree(depth + 1, totalDepth);

        // in top right quadrant
        tempTopLeft.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempTopLeft.z = this.topLeftBound.z;
        tempBottomRight.x = bottomRightBound.x;
        tempBottomRight.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        topRight = new QuadTreeNode(tempTopLeft, tempBottomRight);
        topRight.initializeTree(depth + 1, totalDepth);
    }

    // returns true if node collides with another prism in the tree
    // returns false if node collides with no prism in the tree or node is bad node
    public void insertNode(Prism node)
    {
        if (node == null)
            return;

        // get bounds for the prism node
        Vector3 topLeftBound = getTopLeftOfNode(node);
        Vector3 bottomRightBound = getBottomRightOfNode(node);

        // in top left quadrant
        if (topLeft != null)
        {
            if (pointIn(topLeftBound, bottomRightBound, topLeft.getTopLeftBound(), topLeft.getBottomRightBound()))
            {
                topLeft.insertNode(node);
            }
        }

        // in bottom right quadrant
        if (bottomRight != null)
        {
            if (pointIn(topLeftBound, bottomRightBound, bottomRight.getTopLeftBound(), bottomRight.getBottomRightBound()))
            {
                bottomRight.insertNode(node);
            }
        }

        // in bottom left quadrant
        if (bottomLeft != null)
        {
            if (pointIn(topLeftBound, bottomRightBound, bottomLeft.getTopLeftBound(), bottomLeft.getBottomRightBound()))
            {
                bottomLeft.insertNode(node);
            }
        }

        // in top right quadrant
        if (topRight != null)
        {
            if (pointIn(topLeftBound, bottomRightBound, topRight.getTopLeftBound(), topRight.getBottomRightBound()))
            {
                topRight.insertNode(node);
            }
        }

        prisms.Add(node);
    }

    // checks whether the four corners of the prism square are inside the quadrant
    bool pointIn(Vector3 prismTopLeft, Vector3 prismBottomRight, Vector3 quadTopLeft, Vector3 quadBottomRight)
    {
        Vector3 prismTopRight = Vector3.zero;
        prismTopRight.x = prismBottomRight.x;
        prismTopRight.z = prismTopLeft.z;

        Vector3 prismBottomLeft = Vector3.zero;
        prismBottomLeft.x = prismTopLeft.x;
        prismBottomLeft.z = prismBottomRight.z;

        // tests top left point of prism
        if (prismTopLeft.x >= quadTopLeft.x && prismTopLeft.x <= quadBottomRight.x &&
            prismTopLeft.z <= quadTopLeft.z && prismTopLeft.z >= quadBottomRight.z)
        {
            return true;
        }

        // tests bottom right point of prism
        if (prismBottomRight.x >= quadTopLeft.x && prismBottomRight.x <= quadBottomRight.x &&
            prismBottomRight.z <= quadTopLeft.z && prismBottomRight.z >= quadBottomRight.z)
        {
            return true;
        }

        // tests top right point of prism
        if (prismTopRight.x >= quadTopLeft.x && prismTopRight.x <= quadBottomRight.x &&
            prismTopRight.z <= quadTopLeft.z && prismTopRight.z >= quadBottomRight.z)
        {
            return true;
        }

        // tests bottom left point of prism
        if (prismBottomLeft.x >= quadTopLeft.x && prismBottomLeft.x <= quadBottomRight.x &&
            prismBottomLeft.z <= quadTopLeft.z && prismBottomLeft.z >= quadBottomRight.z)
        {
            return true;
        }

        return false; // prism vectors not in quadrant
    }

    public List<Prism> getCollisionList(Prism node)
    {
        List<Prism> collisions = new List<Prism>();
        collisions.Add(node);

        bool hasNode = false;
        for (int i = 0; i < prisms.Count; i++)
        {
            if (samePrism(prisms[i], node))
            {
                hasNode = true;
                break;
            }
        }
        
        // prism is not in the quadrant
        if (hasNode == false)
        {
            return collisions;
        }

        // removes the searched node from the prisms list to improve prism manager efficiency
        prisms.Remove(node);

        // deepest node in the tree
        if (topLeft == null)
        {
            return prisms;
        }

        List<Prism> results = new List<Prism>();
        results = topLeft.getCollisionList(node);
        if (results != null)
        {
            collisions.AddRange(results);
            results = null; // reset the results list
        }

        results = bottomRight.getCollisionList(node);
        if (results != null)
        {
            collisions.AddRange(results);
            results = null; // reset the results list
        }

        results = bottomLeft.getCollisionList(node);
        if (results != null)
        {
            collisions.AddRange(results);
            results = null; // reset the results list
        }

        results = topRight.getCollisionList(node);
        if (results != null)
        {
            collisions.AddRange(results);
            results = null; // reset the results list
        }

        return collisions;
    }

    /*
    // returns quadtree node with the prism that collides with target
    // returns null if the node is not in the QuadTree
    public QuadTreeNode hasCollision(Prism node)
    {
        
        // point is not inside the bounds of this QuadTree
        if (!inBounds(point))
            return null;

        if (node != null)
            return node;

        
        QuadTreeNode result = null;

        // searches child nodes for prism
        if (topLeft != null)
        {
            if (samePrism(node, topLeft.node)) // parent and child are colliding
            {
                return this;
            }
            result = hasCollision(node);
        }

        if (bottomRight != null)
        {
            if (samePrism(node, bottomRight.node)) // parent and child are colliding
            {
                return this;
            }
            result = hasCollision(node);
        }

        if (bottomLeft != null)
        {
            if (samePrism(node, bottomLeft.node)) // parent and child are colliding
            {
                return this;
            }
            result = hasCollision(node);
        }

        if (topRight != null)
        {
            if (samePrism(node, topRight.node)) // parent and child are colliding
            {
                return this;
            }
            result = hasCollision(node);
        }

        // null if prism not in tree
        return result;
    }

    
    bool inBounds(Prism node)
    {
        Vector3 topLeftBound = getTopLeftOfNode(node);
        Vector3 bottomRightBound = getBottomRightOfNode(node);

        if (topLeftBound.x >= this.topLeftBound.x && bottomRightBound.x <= this.bottomRightBound.x &&
            topLeftBound.z <= this.topLeftBound.z && bottomRightBound.z >= this.bottomRightBound.z)
        {
            return true;
        }
        return false;
    }
    */
    public Vector3 getTopLeftBound()
    {
        return this.topLeftBound;
    }

    public Vector3 getBottomRightBound()
    {
        return this.bottomRightBound;
    }
    

    // returns the top left bound of the prism (max)
    Vector3 getTopLeftOfNode(Prism node)
    {
        Vector3 topLeft = node.points[0];
        foreach (Vector3 point in node.points)
        {
            if (point.x < topLeft.x)
                topLeft.x = point.x;

            if (point.z > topLeft.z)
                topLeft.z = point.z;
        }
        return topLeft;
    }

    // returns the bottom right bound of the prism (min)
    Vector3 getBottomRightOfNode(Prism node)
    {
        Vector3 bottomRight = node.points[0];
        foreach (Vector3 point in node.points)
        {
            if (point.x > bottomRight.x)
                bottomRight.x = point.x;

            if (point.z < bottomRight.z)
                bottomRight.z = point.z;
        }
        return bottomRight;
    }

    // compares this prism's points to node's points
    public bool samePrism(Prism a, Prism b)
    {
        if (a.points.Length == b.points.Length)
        {
            for (int i = 0; i < a.points.Length; i++)
            {
                // different points
                if (a.points[i] != b.points[i])
                {
                    return false;
                }
            }

            // all points in both prisms are the same
            return true;
        }
        return false;
    }
}
