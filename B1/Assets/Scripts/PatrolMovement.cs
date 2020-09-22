using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public float speed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * speed * Vector3.right;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If statement to make patrol bounce back and forth when collides with a wall.
        if(other.gameObject.CompareTag("Agent"))
        {
            speed *= 1;
        }
        else
        {
            speed *= -1;
        }
    }
}
