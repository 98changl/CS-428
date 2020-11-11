using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float border = 10f;
    public float scrollSpeed = 200f;
    float xAxis, yAxis;

    public AgentManager manage;

    // Update is called once per frame
    void Update()
    {
        cameraLook();
        cameraMovements();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        //Get mouse click position
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                manage.destination = hit.point;
            }

        }
    }

    void cameraLook()
    {
        xAxis += Input.GetAxis("Mouse X");
        yAxis += -Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector2(yAxis, xAxis);
    }

    void cameraMovements()
    {
        //Move Forward
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.TransformDirection(Vector3.forward * 10f * Time.deltaTime);
        }
        //Move Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += transform.TransformDirection(Vector3.left * 10f * Time.deltaTime);
        }
        //Move Back
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += transform.TransformDirection(Vector3.back * 10f * Time.deltaTime);
        }
        //Move Right
        if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += transform.TransformDirection(Vector3.right * 10f * Time.deltaTime);
        }
        //Move Up
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += 10f * new Vector3(0, 1, 0) * Time.deltaTime;
        }
        //Move Down
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += 10f * new Vector3(0, -1, 0) * Time.deltaTime;
        }
    }
}