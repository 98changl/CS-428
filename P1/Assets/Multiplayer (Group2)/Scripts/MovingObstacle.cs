using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    // 0 - Movement along x axis
    // 1 - Movement along z axis
    public int direction;
    public int speed;
    public float distance;

    float point1;
    float point2;

    private void Start()
    {
        point1 = transform.position.x;
        point2 = transform.position.z;
    }

    void Update()
    {

        if(direction == 0)
        {
            transform.position = new Vector3(Mathf.PingPong(Time.time * speed, distance) + point1, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.PingPong(Time.time * speed, distance) + point2);
        }

    }
}
