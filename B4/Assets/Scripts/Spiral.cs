using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiral : MonoBehaviour
{
    public AgentManager manage;

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
        manage.destination = new Vector3(0, 0, 0);

    }
}
