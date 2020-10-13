using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float border = 10f;
    public float scrollSpeed = 200f;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        // camera "sprint"
        if (Input.GetKey("left shift"))
        {
            moveSpeed = 20f;
        }
        else
        {
            moveSpeed = 10f;
        }

        // forward
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - border)
        {
            pos.z += moveSpeed * Time.deltaTime;
        }
        // back
        if (Input.GetKey("s") || Input.mousePosition.y <= border)
        {
            pos.z -= moveSpeed * Time.deltaTime;
        }
        // left
        if (Input.GetKey("a") || Input.mousePosition.x <= border)
        {
            pos.x -= moveSpeed * Time.deltaTime;
        }
        // right
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - border)
        {
            pos.x += moveSpeed * Time.deltaTime;
        }

        // scroll up and down
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.deltaTime;
        
        transform.position = pos;
    }

}
