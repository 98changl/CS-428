using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private float jumpAnimationBlend = 0;
    private float xAxis;
    private float gravity = 3f;
    private Vector3 moveVector = Vector3.zero;

    Animator animator;
    CharacterController Controller;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
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

        if(LeftRight == 0 && ForwardBackward == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

        Debug.Log(ForwardBackward * speed * .12f);

        animator.SetFloat("x", LeftRight, .1f, Time.deltaTime);
        animator.SetFloat("y", ForwardBackward * speed * .12f, .1f, Time.deltaTime);

        //Translate inputs to a vector and normalize movement speed
        Vector3 Movement = new Vector3(LeftRight, 3, ForwardBackward);
        Movement *= speed;
        Movement.Normalize();

        transform.localPosition += transform.TransformDirection(Movement * speed * Time.deltaTime);


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

        if (Controller.isGrounded)
        {
            animator.SetBool("isAirborne", false);
            jumpAnimationBlend = 0;
        }
        else
        {
            jumpAnimationBlend += 0.2f * Time.deltaTime;
            animator.SetBool("isAirborne", true);
            animator.SetFloat("Jump Blend", jumpAnimationBlend);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 8;
        }
        else
        {
            speed = 4;
        }

    }


}
