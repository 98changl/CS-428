using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 6f;
    public bool onGround;
    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        onGround = true;

    }


    //Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Translate(moveHorizontal, 0.0f, moveVertical);
        
        if (Input.GetButton("Jump") && onGround == true)
        {
            rb.velocity = new Vector3(0.0f, jumpForce, 0.0f);
            onGround = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ground") || other.gameObject.CompareTag("plataform"))
        {
            onGround = true;
        }
    }
}

