using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiral : MonoBehaviour
{
    public float T = -10;
    public AgentManager manage;
    public float adder = 0;
    public float radius = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        var top1 = Mathf.Pow((0.5f * (1 + Mathf.Sqrt(5))), T);
        var top2 = Mathf.Pow((2 / ((1 + Mathf.Sqrt(5)))), T);

        var X = (-1 * ((top1 - top2 * Mathf.Cos(Mathf.PI * T)) / Mathf.Sqrt(5)));

        T += .0005f;
        */
        //Mathf.Cos(Mathf.PI * T)


        var sinCurve = Mathf.Sin(adder) * radius;
        var cosCurve = Mathf.Cos(adder) * radius;

        manage.destination = new Vector3(0, 0, 0);

        adder += 0.3f * Time.deltaTime;
        radius += 0.001f;

        Debug.Log(sinCurve);

    }
}
