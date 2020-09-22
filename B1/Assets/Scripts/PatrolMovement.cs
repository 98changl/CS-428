using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    float speed = 2.0f;
    void Update()
    {
        transform.position += Time.deltaTime * speed * Vector3.right;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("agent"))
        {
            speed = speed * 1;
        }
        else
        {
            speed = speed * -1;
        }
    }
}
