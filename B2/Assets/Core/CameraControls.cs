using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Start is called before the first frame update

    float xAxis, yAxis;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Lets camera look aroumd.
        cameraLook();

        //Lets camera move around the space.
        cameraMovements();

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
            transform.localPosition += transform.TransformDirection(Vector3.forward * 0.07f);
        }
        //Move Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += transform.TransformDirection(Vector3.left * 0.07f);
        }
        //Move Back
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += transform.TransformDirection(Vector3.back * 0.07f);
        }
        //Move Right
        if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += transform.TransformDirection(Vector3.right * 0.07f);
        }
        //Move Up
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += 0.05f * new Vector3(0, 1, 0);
        }
        //Move Down
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += 0.05f * new Vector3(0, -1, 0);
        }
    }





}
