using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public float speed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * speed * Vector3.right;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("player") || other.gameObject.CompareTag("wall"))
        {
            speed *= 1;
        }
        else
        {
            speed *= -1;
        }
    }
}
