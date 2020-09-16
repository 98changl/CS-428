using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    
    // forces
    public float speed;
    public float jumpForce;

    // text
    public Text countText;
    //public Text winText;
    public string color;

    public LayerMask ground;
    public SphereCollider col;

    // controls
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;

    public GameObject PickUps;

    private Rigidbody rb;
    private int count;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();

        count = 0;
        SetCountText();
        //winText.text = "";
    }

    void FixedUpdate ()
    {
        float moveHorizontal = 0;
        float moveVertical = 0;

        if (Input.GetKey(up) || Input.GetKey(down))
        {
            moveVertical = Input.GetAxis("Vertical");
        }

        if (Input.GetKey(left) || Input.GetKey(right))
        {
            moveHorizontal = Input.GetAxis("Horizontal");
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);

        if (IsGrounded() && Input.GetKey(jump))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Pick Up"))
        {
            other.gameObject.SetActive(false);
            PickUps.GetComponent<Respawner>().SpawnFunction(); // calls the respawner

            count = count + 1;
            SetCountText();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            count = count - 1;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (transform.position.y > other.gameObject.transform.position.y)
            {
                count = count + 1;
            }

            if (transform.position.y < other.gameObject.transform.position.y)
            {
                count = count - 1;
            }
        }

        SetCountText();
    }
    
    void SetCountText()
    {
        if (count <= 0)
        {
            count = 0;
        }

        countText.text = color + ": " + count.ToString();
        /*
        if (count >= 12)
        {
            winText.text = "You Win!";
        }
        */
    }
    
    private bool IsGrounded()
    {
        return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z), col.radius * .9f, ground);
    }

}
