using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float speed;
    public float jumpForce = 5f;
    public bool onGround;
    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        onGround = true;

    }

    private void Update()
    {
        moveObstacle();
    }

    //Update is called once per frame
    //void FixedUpdate()
    //{
    //    float moveHorizontal = Input.GetAxis("Horizontal");
    //    float moveVertical = Input.GetAxis("Vertical");

    //    Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
    //    rb.AddForce(movement * speed);
    //
    //  if( Input.GetButton("Jump") && onGround == true)
    //{
    //   rb.velocity = new Vector3(0.0f, jumpForce, 0.0f);
    //    onGround = false;
    //   }
    // }

    void moveObstacle()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            onGround = true;
        }
    }
}

