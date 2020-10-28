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
        Vector3 topLeftBound = new Vector3(-prismRegionRadiusXZ - 1, 0, prismRegionRadiusXZ + 1);
        Vector3 bottomRightBound = new Vector3(prismRegionRadiusXZ + 1, 0, -prismRegionRadiusXZ - 1);

        //Debug.Log("Top left:" + topLeftBound);
        //Debug.Log("Bot right:" + bottomRightBound);

        QuadTreeNode root = new QuadTreeNode(topLeftBound, bottomRightBound);
        root.initializeTree(0, 3); // create all quadrants of the tree

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
                if (!root.samePrism(prisms[i], collisions[j])) // this currently has duplicate collisions
                {
                    var checkPrisms = new PrismCollision();
                    checkPrisms.a = prisms[i];
                    checkPrisms.b = collisions[j];
                    //Debug.Log("Collision found");
                    yield return checkPrisms;
                }
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

        List<Vector3> points = new List<Vector3>();
        
        //Minkowski Difference--------------------------
        foreach(var point1 in prismA.points)
            foreach(var point2 in prismB.points)
            {
                var result = Vector3.zero;
                result = point1 - point2;
                points.Add(result);
            }
        //----------------------------------------------

        var simplex = new List<Vector3>();
        simplex.Add(points.Aggregate((a,b) => a.x < b.x ? a : b));
        simplex.Add(points.Where(p => p != simplex[0]).Aggregate((a, b) => a.x > b.x ? a : b));

        var intersecting = false;
        do
        {

            if(simplex.Count == 3)
            {
                var simplexOrientation = Mathf.Sign(Vector3.Dot(Vector3.Cross(simplex[1] - simplex[0], Vector3.up), simplex[2] - simplex[0]));

                var bounded = true;
                Vector3? deletePoint = simplex[0];
                for(int i = 0; i < 3; i++)
                {
                    var a = simplex[i];
                    var b = simplex[(i + 1) % 3];

                    var temp = Mathf.Sign(Vector3.Dot(Vector3.Cross(b-a, Vector3.up), Vector3.zero - a));
                    if(temp != simplexOrientation)
                    {
                        bounded = false;
                        deletePoint = simplex[(i + 2) % 3];
                        break;
                    }
                }

                if(bounded == true)
                {
                    intersecting = true;
                    break;
                }

                if (deletePoint.HasValue)
                {
                    simplex.Remove(deletePoint.Value);
                }

            }

            var dir = simplex[1] - simplex[0];
            var tangent = Vector3.Cross(dir, Vector3.up);
            var orientation = Mathf.Sign(Vector3.Dot(tangent, -simplex[0]));
            var supportAxis = tangent * orientation;
            var supportPoint = points.Aggregate((a, b) => Vector3.Dot(a, supportAxis) > Vector3.Dot(b, supportAxis) ? a : b);

            if (!simplex.Contains(supportPoint))
            {
                simplex.Add(supportPoint);
            }


        }
        while (simplex.Count == 3);

        //Returns------------------------------------------------
        collision.penetrationDepthVectorAB = Vector3.zero;
        if(intersecting == true)
        {
            return true;
        }
        else
        {
            return false;
        }
        //--------------------------------------------------------
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
