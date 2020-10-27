using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeNode : MonoBehaviour
{
    // Bounding Box (AABB)
    Vector3 topLeftBound;       // max
    Vector3 bottomRightBound;   // min

    // Prism node
    public Prism node;

    // QuadTree child nodes
    QuadTreeNode topLeft;
    QuadTreeNode topRight;
    QuadTreeNode bottomLeft;
    QuadTreeNode bottomRight;

    public QuadTreeNode(Prism node)
    {
        this.topLeftBound = getTopLeftOfNode(node);
        this.bottomRightBound = getBottomRightOfNode(node);
        this.node = node;

        this.topLeft = null;
        this.topRight = null;
        this.bottomLeft = null;
        this.bottomRight = null;
    }

    // returns true if node collides with another prism in the tree
    // returns false if node collides with no prism in the tree or node is bad node
    public bool insertNode(Prism node)
    {
        if (node == null)
            return false;

        //bool result = false;
        Vector3 topLeftBound = getTopLeftOfNode(node);
        Vector3 bottomRightBound = getBottomRightOfNode(node);

        Vector3 tempTopLeft = Vector3.zero;
        Vector3 tempBottomRight = Vector3.zero;

        // in top left quadrant
        tempTopLeft = this.topLeftBound;
        tempBottomRight.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempBottomRight.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        if (pointIn(topLeftBound, bottomRightBound, tempTopLeft, tempBottomRight))
        {
            if (topLeft == null)
            {
                topLeft = new QuadTreeNode(node);
            }
            else
            {
                topLeft.insertNode(node);
            }
            return true;
        }

        // in bottom right quadrant
        tempTopLeft.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempTopLeft.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        tempBottomRight = this.bottomRightBound;
        if (pointIn(topLeftBound, bottomRightBound, tempTopLeft, tempBottomRight))
        {
            if (bottomRight == null)
            {
                bottomRight = new QuadTreeNode(node);
            }
            else
            {
                bottomRight.insertNode(node);
            }
            return true;
        }

        // in bottom left quadrant
        tempTopLeft.x = topLeftBound.x;
        tempTopLeft.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        tempBottomRight.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempBottomRight.z = this.bottomRightBound.z;
        if (pointIn(topLeftBound, bottomRightBound, tempTopLeft, tempBottomRight))
        {
            if (bottomLeft == null)
            {
                bottomLeft = new QuadTreeNode(node);
            }
            else
            {
                bottomLeft.insertNode(node);
            }
            return true;
        }

        // in top right quadrant
        tempTopLeft.x = (this.topLeftBound.x + this.bottomRightBound.x) / 2;
        tempTopLeft.z = this.topLeftBound.z;
        tempBottomRight.x = bottomRightBound.x;
        tempBottomRight.z = (this.topLeftBound.z + this.bottomRightBound.z) / 2;
        if (pointIn(topLeftBound, bottomRightBound, tempTopLeft, tempBottomRight))
        {
            if (topRight == null)
            {
                topRight = new QuadTreeNode(node);
            }
            else
            {
                topRight.insertNode(node);
            }
            return true;
        }

        // prism does not collide with current prism
        // checks if the node collides with a node colliding with this node

        bool result = false;
        if (topLeft != null)
        {
            result = topLeft.insertNode(node);
        }

        if (result == true) // only one insert is performed
            return result;

        if (bottomLeft != null)
        {
            result = bottomLeft.insertNode(node);
        }

        if (result == true) // only one insert is performed
            return result;

        if (topRight != null)
        {
            result = topRight.insertNode(node);
        }

        if (result == true) // only one insert is performed
            return result;

        if (bottomRight != null)
        {
            result = bottomRight.insertNode(node);
        }

        return result;
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

    // returns quadtree node with the prism that collides with target
    // returns null if the node is not in the QuadTree
    public QuadTreeNode hasCollision(Prism node)
    {
        /*
        // point is not inside the bounds of this QuadTree
        if (!inBounds(point))
            return null;

        if (node != null)
            return node;

        */
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

    /*
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

    Vector3 getTopLeftBound()
    {
        return this.topLeftBound;
    }

    Vector3 getBottomRightBound()
    {
        return this.bottomRightBound;
    }
    */

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
    bool samePrism(Prism a, Prism b)
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
