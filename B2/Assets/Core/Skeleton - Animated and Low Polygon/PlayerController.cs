using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private float xAxis;

    private float movementSpeed = 2f;
    private float curentSpeed = 0f;
    private float speedSmoothVelocity = 0f;
    private float speedSmoothTime = 0.1f;
    private float rotationSpeed = 0.1f;
    private float gravity = 3f;
    private Vector3 moveVector = Vector3.zero;

    private Transform mainCameraTransform;

    Animator animator;
    CharacterController Controller;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();
    }

    private void Look()
    {
        xAxis += Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector2(0, xAxis);
    }

    private void Move()
    {

        //Scan axis for inputs
        float LeftRight = Input.GetAxisRaw("Horizontal");
        float ForwardBackward = Input.GetAxisRaw("Vertical");


        //Translate inputs to a vector and normalize movement speed
        Vector3 Movement = new Vector3(LeftRight, 3, ForwardBackward);
        Movement.Normalize();


        transform.localPosition += transform.TransformDirection(Movement * speed * Time.deltaTime);

        //Vector3 moveVector;
        //moveVector = Vector3.zero;

        //Check if character is grounded and add to gravity if it isnt
        if (Controller.isGrounded == false)
        {
            moveVector += Physics.gravity;
        }

        Controller.Move(moveVector * Time.deltaTime * 0.01f);

        //Get character to jump if he is on the ground and space is pressed.
        if (Input.GetKeyDown(KeyCode.Space) && Controller.isGrounded)
        {
            moveVector = Vector3.zero;
            Vector3 Jump = new Vector3(0, 5, 0);
            Jump.Normalize();
            transform.localPosition += transform.TransformDirection(Jump * 30 * Time.deltaTime);

        }




        Debug.Log(Controller.isGrounded);
    }


}
