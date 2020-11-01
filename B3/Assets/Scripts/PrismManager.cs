using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrismManager : MonoBehaviour
{
    public int prismCount = 10;
    public float prismRegionRadiusXZ = 5;
    public float prismRegionRadiusY = 5;
    public float maxPrismScaleXZ = 5;
    public float maxPrismScaleY = 5;
    public GameObject regularPrismPrefab;
    public GameObject irregularPrismPrefab;

    private List<Prism> prisms = new List<Prism>();
    private List<GameObject> prismObjects = new List<GameObject>();
    private GameObject prismParent;
    private Dictionary<Prism,bool> prismColliding = new Dictionary<Prism, bool>();

    private const float UPDATE_RATE = 0.5f;

    #region Unity Functions

    void Start()
    {
        Random.InitState(0);    //10 for no collision

        prismParent = GameObject.Find("Prisms");
        for (int i = 0; i < prismCount; i++)
        {
            var randPointCount = Mathf.RoundToInt(3 + Random.value * 7);
            var randYRot = Random.value * 360;
            var randScale = new Vector3((Random.value - 0.5f) * 2 * maxPrismScaleXZ, (Random.value - 0.5f) * 2 * maxPrismScaleY, (Random.value - 0.5f) * 2 * maxPrismScaleXZ);
            var randPos = new Vector3((Random.value - 0.5f) * 2 * prismRegionRadiusXZ, (Random.value - 0.5f) * 2 * prismRegionRadiusY, (Random.value - 0.5f) * 2 * prismRegionRadiusXZ);

            GameObject prism = null;
            Prism prismScript = null;
            if (Random.value < 0.5f)
            {
                prism = Instantiate(regularPrismPrefab, randPos, Quaternion.Euler(0, randYRot, 0));
                prismScript = prism.GetComponent<RegularPrism>();
            }
            else
            {
                prism = Instantiate(irregularPrismPrefab, randPos, Quaternion.Euler(0, randYRot, 0));
                prismScript = prism.GetComponent<IrregularPrism>();
            }
            prism.name = "Prism " + i;
            prism.transform.localScale = randScale;
            prism.transform.parent = prismParent.transform;
            prismScript.pointCount = randPointCount;
            prismScript.prismObject = prism;

            prisms.Add(prismScript);
            prismObjects.Add(prism);
            prismColliding.Add(prismScript, false);

        }

        StartCoroutine(Run());
    }
    
    void Update()
    {
        #region Visualization

        DrawPrismRegion();
        DrawPrismWireFrames();

#if UNITY_EDITOR
        if (Application.isFocused)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
#endif

        #endregion

        Debug.Log(prisms[1].points[1]);

    }

    IEnumerator Run()
    {
        yield return null;

        while (true)
        {
            foreach (var prism in prisms)
            {
                prismColliding[prism] = false;
            }

            foreach (var collision in PotentialCollisions())
            {
                if (CheckCollision(collision))
                {
                    prismColliding[collision.a] = true;
                    prismColliding[collision.b] = true;

                    ResolveCollision(collision);
                }
            }

            yield return new WaitForSeconds(UPDATE_RATE);
        }
    }

    #endregion

    #region Incomplete Functions

    private IEnumerable<PrismCollision> PotentialCollisions()
    {
        // quad tree data structure implementation
        Vector3 topLeftBound = new Vector3(-prismRegionRadiusXZ - 2, 0, prismRegionRadiusXZ + 2);
        Vector3 bottomRightBound = new Vector3(prismRegionRadiusXZ + 2, 0, -prismRegionRadiusXZ - 2);

        //Debug.Log("Top left:" + topLeftBound);
        //Debug.Log("Bot right:" + bottomRightBound);

        QuadTreeNode root = new QuadTreeNode(topLeftBound, bottomRightBound);

        // determine the depth of the coordinate grid for the QuadTree
        int depth = 0;
        while(true)
        {
            if (Mathf.Pow(4, depth) < prismCount)
            {
                depth = depth + 1;
            }
            else
            {
                break;
            }
        }
        root.initializeTree(0, depth); // create all quadrants of the tree

        // inserts prisms into tree
        for (int i = 0; i < prisms.Count; i++)
        {
            root.insertNode(prisms[i]);
        }

        // checks for collisions
        for (int i = 0; i < prisms.Count; i++)
        {
            List<Prism> collisions = new List<Prism>();
            collisions = root.getCollisionList(prisms[i]);

            // compare prism[i] to list of collisions retrieved from root
            for (int j = 0; j < collisions.Count; j++)
            {
                var checkPrisms = new PrismCollision();
                checkPrisms.a = prisms[i];
                checkPrisms.b = collisions[j];
                yield return checkPrisms;
            }
        }
        yield break;
    }

    private bool CheckCollision(PrismCollision collision)
    {
        var prismA = collision.a;
        var prismB = collision.b;

        List<Vector3> pointList = new List<Vector3>();

        //Minkowski Difference--------------------------
        int pointsA = prismA.pointCount;
        int pointsB = prismB.pointCount;
        //Find the difference between the points in two potentially colliding polygons
        for (int i = 0; i < pointsA; i++)
        {
            for (int j = 0; j < pointsB; j++)
            {
                var result = Vector3.zero;
                result = prismA.points[i] - prismB.points[j];
                pointList.Add(result);
            }
        }
        //----------------------------------------------

        //GJK-------------------------------------------

        //Initialize simplex
        var simplex = new List<Vector3>();
        simplex.Add(pointList.Aggregate((point1, point2) => point1.x < point2.x ? point1 : point2));
        simplex.Add(pointList.Where(p => p != simplex[0]).Aggregate((point1, point2) => point1.x > point2.x ? point1 : point2));

        bool isIntersecting = false;

        int counter = 3;
        while (counter == 3)
        {
            if (simplex.Count == 3)
            {
                var simplexOrientation = Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), simplex[2] - simplex[0]));
                //Compare orientation of the simplex to the orientation of the dot product of the different points in the simplex
                bool contains = true;
                Vector3? pointRemoval = simplex[0];
                for (int i = 0; i < 3; i++)
                {
                    //If the orientations are not equal then the point is removed from the simplex
                    var orientationCompare = Mathf.Sign(Vector3.Dot(Vector3.Cross(getNextArrayPoint(simplex, i, 1) - simplex[i], Vector3.up), Vector3.zero - simplex[i]));
                    if (orientationCompare != simplexOrientation)
                    {
                        pointRemoval = getNextArrayPoint(simplex, i, 2);
                        contains = false;
                    }
                }
                //If the origin is contained then set 
                if (contains == true)
                {
                    isIntersecting = true;
                    break;
                }
                //Remove point from simplex
                if (pointRemoval.Value != null)
                {
                    simplex.Remove(pointRemoval.Value);
                }
            }
            //Add support point into simplex when found
            if (!simplex.Contains(FindSupportPoint(pointList, simplex)))
            {
                simplex.Add(FindSupportPoint(pointList, simplex));
            }

            counter = simplex.Count;

        }
        //----------------------------------------------*

        //EPA-------------------------------------------

        //Create new movement vector to move prisms
        Vector3 movement = new Vector3();
        
        //If the prisms arent intersecting dont compute penetration depth
        if (isIntersecting != false)
        {
            var tanToLine = Vector3.Cross((simplex[2]-simplex[1]), Vector3.up);
            var product = Vector3.Dot(simplex[0]-simplex[1], tanToLine.normalized);

            //Switch simplex points if dot product is greater than 0
            if (product > 0)
            {
                var holder = simplex[0];
                simplex[0] = simplex[1];
                simplex[1] = holder;
            }

            //Find triple product of simplex points
            List<float> lineSegmentDistance = new List<float>();
            for (int i = 0; i < 3; i++)
            {
                tanToLine = Vector3.Cross((getNextArrayPoint(simplex, i, 1) - simplex[i]), Vector3.up);
                product = Vector3.Dot(Vector3.zero - simplex[i], tanToLine.normalized);
                lineSegmentDistance.Add(Mathf.Abs(product));
            }

            //Find index of the smallest float point
            int lowest = 0;
            float current = lineSegmentDistance[0];
            for (int i = 0; i < lineSegmentDistance.Count; i++)
            {
                if (lineSegmentDistance[i] < current)
                {
                    current = lineSegmentDistance[i];
                    lowest = i;
                }
            }
            int SmallestIndexPoint = lowest;

            while(true)
            {
                //Break out of infinite loop if the support point is contained in the simplex
                if (simplex.Contains(epaSupportPoint(pointList, simplex, SmallestIndexPoint)))
                {
                    break;
                }
                else
                {
                    //Insert support point into simplex if its not contained already
                    int point = (SmallestIndexPoint + 1) % simplex.Count;
                    simplex.Insert(point, epaSupportPoint(pointList, simplex, SmallestIndexPoint));
                    lineSegmentDistance.Insert(point, float.MaxValue);
                    //Find index of the smallest float point
                    lowest = 0;
                    current = lineSegmentDistance[0];
                    for (int i = 0; i < lineSegmentDistance.Count; i++)
                    {
                        if (lineSegmentDistance[i] < current)
                        {
                            current = lineSegmentDistance[i];
                            lowest = i;
                        }
                    }
                    SmallestIndexPoint = lowest;

                    //Find the dot product of the simplex from smallest index points to compute new minimum distance
                    for (int j = SmallestIndexPoint; j <= SmallestIndexPoint + 1; j++)
                    {
                        tanToLine = Vector3.Cross((simplex[(j + 1) % simplex.Count] - simplex[(j) % simplex.Count]), Vector3.up);
                        product = Vector3.Dot(Vector3.zero - simplex[(j) % simplex.Count], tanToLine.normalized);
                        lineSegmentDistance[(j) % simplex.Count] = Mathf.Abs(product);
                    }

                    //Recalculate smallest index point for the movement vector.
                    lowest = 0;
                    current = lineSegmentDistance[0];
                    for (int i = 0; i < lineSegmentDistance.Count; i++)
                    {
                        if (lineSegmentDistance[i] < current)
                        {
                            current = lineSegmentDistance[i];
                            lowest = i;
                        }
                    }

                    SmallestIndexPoint = lowest;
                    //Set movement vector to the penetration depth of the prisms
                    movement = simplex[SmallestIndexPoint] + getNextArrayPoint(simplex, SmallestIndexPoint, 1) / 2;
                }
            }
        }
        //Returns------------------------------------------------
        if (isIntersecting == true)
        {
            collision.penetrationDepthVectorAB = movement * 1.444f;
            return true;
        }
        else
        {
            collision.penetrationDepthVectorAB = Vector3.zero;
            return false;
        }
        //--------------------------------------------------------
    }

    private Vector3 getNextArrayPoint(List<Vector3> array, int currentPoisition, int desiredPositionForward )
    {
        int counter = array.Count;
        Vector3 nextPosition = array[(currentPoisition + desiredPositionForward) % counter];
        return nextPosition;
    }

    private Vector3 epaSupportPoint(List<Vector3> pointList, List<Vector3> simplex, int SmallestIndexPoint)
    {
        var tanToLine = Vector3.Cross(simplex[(SmallestIndexPoint + 1) % simplex.Count] - simplex[SmallestIndexPoint], Vector3.up);
        var orientation = -Mathf.Sign(Vector3.Dot(tanToLine, -simplex[SmallestIndexPoint]));
        var axis = (Vector3.Cross(simplex[(SmallestIndexPoint + 1) % simplex.Count] - simplex[SmallestIndexPoint], Vector3.up)) * (-Mathf.Sign(Vector3.Dot(tanToLine, -simplex[SmallestIndexPoint])));
        return pointList.Aggregate((point1, point2) => Vector3.Dot(point1, axis) > Vector3.Dot(point2, axis) ? point1 : point2);
    }

    private Vector3 FindSupportPoint(List<Vector3> points, List<Vector3> simplex)
    {
        var supportAxis = (Vector3.Cross(simplex[1] - simplex[0], Vector3.up)) * (Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), -simplex[0])));
        return points.Aggregate((a, b) => Vector3.Dot(a, supportAxis) > Vector3.Dot(b, supportAxis) ? a : b);
    }
    #endregion

    #region Private Functions

    private void ResolveCollision(PrismCollision collision)
    {
        var prismObjA = collision.a.prismObject;
        var prismObjB = collision.b.prismObject;

        var pushA = -collision.penetrationDepthVectorAB / 2;
        var pushB = collision.penetrationDepthVectorAB / 2;

        for (int i = 0; i < collision.a.pointCount; i++)
        {
            collision.a.points[i] += pushA;
        }
        for (int i = 0; i < collision.b.pointCount; i++)
        {
            collision.b.points[i] += pushB;
        }
        //prismObjA.transform.position += pushA;
        //prismObjB.transform.position += pushB;

        Debug.DrawLine(prismObjA.transform.position, prismObjA.transform.position + collision.penetrationDepthVectorAB, Color.cyan, UPDATE_RATE);
    }
    
    #endregion

    #region Visualization Functions

    private void DrawPrismRegion()
    {
        var points = new Vector3[] { new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1) }.Select(p => p * prismRegionRadiusXZ).ToArray();
        
        var yMin = -prismRegionRadiusY;
        var yMax = prismRegionRadiusY;

        var wireFrameColor = Color.yellow;

        foreach (var point in points)
        {
            Debug.DrawLine(point + Vector3.up * yMin, point + Vector3.up * yMax, wireFrameColor);
        }

        for (int i = 0; i < points.Length; i++)
        {
            Debug.DrawLine(points[i] + Vector3.up * yMin, points[(i + 1) % points.Length] + Vector3.up * yMin, wireFrameColor);
            Debug.DrawLine(points[i] + Vector3.up * yMax, points[(i + 1) % points.Length] + Vector3.up * yMax, wireFrameColor);
        }
    }

    private void DrawPrismWireFrames()
    {
        for (int prismIndex = 0; prismIndex < prisms.Count; prismIndex++)
        {
            var prism = prisms[prismIndex];
            var prismTransform = prismObjects[prismIndex].transform;

            var yMin = prism.midY - prism.height / 2 * prismTransform.localScale.y;
            var yMax = prism.midY + prism.height / 2 * prismTransform.localScale.y;

            var wireFrameColor = prismColliding[prisms[prismIndex]] ? Color.red : Color.green;

            foreach (var point in prism.points)
            {
                Debug.DrawLine(point + Vector3.up * yMin, point + Vector3.up * yMax, wireFrameColor);
            }

            for (int i = 0; i < prism.pointCount; i++)
            {
                Debug.DrawLine(prism.points[i] + Vector3.up * yMin, prism.points[(i + 1) % prism.pointCount] + Vector3.up * yMin, wireFrameColor);
                Debug.DrawLine(prism.points[i] + Vector3.up * yMax, prism.points[(i + 1) % prism.pointCount] + Vector3.up * yMax, wireFrameColor);
            }
        }
    }

    #endregion

    #region Utility Classes

    private class PrismCollision
    {
        public Prism a;
        public Prism b;
        public Vector3 penetrationDepthVectorAB;
    }

    private class Tuple<K,V>
    {
        public K Item1;
        public V Item2;

        public Tuple(K k, V v) {
            Item1 = k;
            Item2 = v;
        }
    }

    #endregion
}
