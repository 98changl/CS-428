using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private float speed = 1.0f;
    private float jumpAnimationBlend = 0;
    private float xAxis;
    private float gravity = 3f;
    private float hitTimer = 0f;
    private Vector3 moveVector = Vector3.zero;

    Animator animator;
    CharacterController Controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }

        //this.name = "Player" + photonView.ViewID;

    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (photonView.IsMine)
        {
            Mover();
            Look();
            Tag();
        }
    }

    private void Look()
    {
        xAxis += Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector2(0, xAxis);
    }

    public void Tag()
    {
        hitTimer += Time.deltaTime;

        if (Input.GetMouseButton(0) && hitTimer > 2)
        {
            animator.SetBool("isClicked", true);
            GetComponent<CapsuleCollider>().radius = 0.6f;
            hitTimer = 0;
        }
        
        if(hitTimer > 2)
        {
            animator.SetBool("isClicked", false);
            GetComponent<CapsuleCollider>().radius = 0.2f;
        }

    }

    private void Mover()
    {
        float LeftRight = Input.GetAxisRaw("Horizontal");
        float ForwardBackward = Input.GetAxisRaw("Vertical");
        //Controller.Move(new Vector3(LeftRight, 0, ForwardBackward) * Time.deltaTime * 20f);

        if (LeftRight == 0 && ForwardBackward == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

        animator.SetFloat("x", LeftRight, .1f, Time.deltaTime);
        animator.SetFloat("y", ForwardBackward * 0.5f * speed, .1f, Time.deltaTime);

        Debug.Log(ForwardBackward);

        Vector3 Movement = new Vector3(LeftRight, 0, ForwardBackward);
        Movement *= speed;
        Movement.Normalize();

        Controller.Move(transform.TransformDirection(Movement * speed * Time.deltaTime));

        if (Controller.isGrounded == false)
        {
            moveVector += Physics.gravity;
        }

        Controller.Move(moveVector * Time.deltaTime * 0.01f);

        if (Input.GetKeyDown(KeyCode.Space) && Controller.isGrounded)
        {
            moveVector = Vector3.zero;
            Vector3 Jump = new Vector3(0, 5, 0);
            Jump.Normalize();
            //Controller.Move(transform.TransformDirection(Jump * 300 * Time.deltaTime));
            moveVector = Vector3.up * 500;

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
            speed = 5;
        }
        else
        {
            speed = 1;
        }

    }
}
