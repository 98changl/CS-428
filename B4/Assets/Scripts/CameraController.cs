using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float border = 10f;
    public float scrollSpeed = 200f;

    public AgentManager manage;

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

        //Get mouse click position
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                manage.SetAgentDestinations(hit.point);
            }
        }

        // forward
        if (Input.GetKey("w"))
        {
            pos.z += moveSpeed * Time.deltaTime;
        }
        // back
        if (Input.GetKey("s"))
        {
            pos.z -= moveSpeed * Time.deltaTime;
        }
        // left
        if (Input.GetKey("a"))
        {
            pos.x -= moveSpeed * Time.deltaTime;
        }
        // right
        if (Input.GetKey("d"))
        {
            pos.x += moveSpeed * Time.deltaTime;
        }

        // scroll up and down
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.deltaTime;
        
        transform.position = pos;
    }

}
