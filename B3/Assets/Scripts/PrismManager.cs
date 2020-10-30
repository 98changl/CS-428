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

        /*
        
        for (int i = 0; i < prisms.Count; i++) {
            for (int j = i + 1; j < prisms.Count; j++) {
                var checkPrisms = new PrismCollision();
                checkPrisms.a = prisms[i];
                checkPrisms.b = prisms[j];

                yield return checkPrisms;
            }
        }
        //        Debug.Log(prisms[1].points[1]);
        yield break;
        */
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
        var simplex = new List<Vector3>();
        simplex.Add(pointList.Aggregate((a, b) => a.x < b.x ? a : b));
        simplex.Add(pointList.Where(p => p != simplex[0]).Aggregate((a, b) => a.x > b.x ? a : b));

        bool isIntersecting = false;
        do
        {
            if (simplex.Count == 3)
            {
                var simplexOrientation = Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), simplex[2] - simplex[0]));

                var bounded = true;
                Vector3? deletePoint = simplex[0];
                for (int i = 0; i < 3; i++)
                {
                    var temp = Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[(i + 1) % 3] - simplex[i], Vector3.up), Vector3.zero - simplex[i]));
                    if (temp != simplexOrientation)
                    {
                        bounded = false;
                        deletePoint = simplex[(i + 2) % 3];
                        break;
                    }
                }
                if (bounded == true)
                {
                    isIntersecting = true;
                    break;
                }
                if (deletePoint.Value != null)
                {
                    simplex.Remove(deletePoint.Value);
                }
            }
            if (!simplex.Contains(FindSupportPoint(pointList, simplex)))
            {
                simplex.Add(FindSupportPoint(pointList, simplex));
            }
        }
        while (simplex.Count == 3);
        //----------------------------------------------

        //EPA-------------------------------------------
        Vector3 movement = new Vector3();

        if (isIntersecting == true)
        {
            if (PointToLine(simplex[0], simplex[1], simplex[2]) > 0)
            {
                var temp = simplex[0];
                simplex[0] = simplex[1];
                simplex[1] = temp;
            }

            var distToSimplexSegments = new List<float>();
            for (int s = 0; s < simplex.Count; s++)
            {
                var a = simplex[s];
                var b = simplex[(s + 1) % simplex.Count];
                distToSimplexSegments.Add(Mathf.Abs(PointToLine(Vector3.zero, a, b)));
            }

            var minIndex = MinIndex(distToSimplexSegments);
            var minDist = distToSimplexSegments[minIndex];

            for (int i = 0; i < 100000; i++)
            {
                if (simplex.Contains(epaSupportPoint(pointList, simplex, minIndex)))
                {
                    break;
                }
                else
                {
                    var ind = (minIndex + 1) % simplex.Count;
                    simplex.Insert(ind, epaSupportPoint(pointList, simplex, minIndex));
                    distToSimplexSegments.Insert(ind, float.MaxValue);

                    minIndex = MinIndex(distToSimplexSegments);

                    for (int s = minIndex; s <= minIndex + 1; s++)
                    {
                        var a = simplex[(s) % simplex.Count];
                        var b = simplex[(s + 1) % simplex.Count];
                        distToSimplexSegments[(s) % simplex.Count] = Mathf.Abs(PointToLine(Vector3.zero, a, b));
                    }
                    minIndex = MinIndex(distToSimplexSegments);
                    minDist = distToSimplexSegments[minIndex];
                    //Output pen depth
                    movement = simplex[minIndex] + simplex[(minIndex + 1) % simplex.Count] / 2;
                }

            }

        }
        //Returns------------------------------------------------
        if (isIntersecting == true)
        {
            collision.penetrationDepthVectorAB = movement * 1.44f;
            return true;
        }
        else
        {
            collision.penetrationDepthVectorAB = Vector3.zero;
            return false;
        }
        //--------------------------------------------------------
    }

    private Vector3 epaSupportPoint(List<Vector3> pointList, List<Vector3> simplex, int minIndex)
    {
        var tangent = Vector3.Cross(simplex[(minIndex + 1) % simplex.Count] - simplex[minIndex], Vector3.up);
        var orientation = -Mathf.Sign(Vector3.Dot(tangent, -simplex[minIndex]));
        var supportAxis = (Vector3.Cross(simplex[(minIndex + 1) % simplex.Count] - simplex[minIndex], Vector3.up)) * (-Mathf.Sign(Vector3.Dot(tangent, -simplex[minIndex])));
        return pointList.Aggregate((a, b) => Vector3.Dot(a, supportAxis) > Vector3.Dot(b, supportAxis) ? a : b);
    }

    private Vector3 FindSupportPoint(List<Vector3> points, List<Vector3> simplex)
    {
        var supportAxis = (Vector3.Cross(simplex[1] - simplex[0], Vector3.up)) * (Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), -simplex[0])));
        return points.Aggregate((a, b) => Vector3.Dot(a, supportAxis) > Vector3.Dot(b, supportAxis) ? a : b);
    }

    private Vector3 PointToLineTangent(Vector3 p, Vector3 a, Vector3 b)
    {
        var newVec = p - a;
        var dir = b - a;
        var tangent = Vector3.Cross(dir, Vector3.up);

        var result = Vector3.Dot(newVec, tangent) / (newVec.magnitude) * newVec.magnitude;
        return tangent * result;
    }

    private float PointToLine(Vector3 p, Vector3 a, Vector3 b)
    {
        var newVec = p - a;
        var dir = b - a;
        var tangent = Vector3.Cross(dir, Vector3.up).normalized;

        var result = Vector3.Dot(newVec, tangent) / (newVec.magnitude) * newVec.magnitude;
        return result;
    }
    private int MinIndex(List<float> a)
    {
        int lowest = 0;
        float current = a[0];
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] < current)
            {
                current = a[i];
                lowest = i;
            }
        }
        return lowest;
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
