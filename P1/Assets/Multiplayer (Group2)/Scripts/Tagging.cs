using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tagging : MonoBehaviour
{
    public Material agentIsNotIt;
    public Material agentIsIt;
    public Renderer agentColor;
    public GameObject self;

    //public float score;
    public bool tagged;

    private bool canBeTagged;
    private float tagDelay = 3f;

    Animator animator;
    CharacterController Controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        //tagged = false;
        canBeTagged = true;
        //WaitForTagAbility();
        //self.parent = GameObject.Find("PlayerManager");
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateStatus();
        if (animator.GetBool("isClicked"))
        {
            GetComponent<CapsuleCollider>().radius = 0.6f;
        }
        else
        {
            GetComponent<CapsuleCollider>().radius = 0.22f;
        }
    }

    private void UpdateStatus()
    {
        if (tagged == true)
        {
            agentColor.material = agentIsIt;
        }
        else
        {
            agentColor.material = agentIsNotIt;
        }
    }

    public void SetTag(bool tag)
    {
        tagged = tag;
        UpdateStatus();
    }

    public bool GetTag()
    {
        return tagged;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.tag == "Player" && canBeTagged == true)
        {
            Debug.Log("Player Trigger");
            if (tagged == false)
            {
                tagged = true;
                UpdateStatus();
            }
            else
            {
                tagged = false;
                UpdateStatus();
            }

            canBeTagged = false;
            StartCoroutine(WaitForTagAbility());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.tag == "Player" && canBeTagged == true)
        {
            Debug.Log("Player Collision");
            if (tagged == false)
            {
                tagged = true;
                UpdateStatus();
            }
            else
            {
                tagged = false;
                UpdateStatus();
            }

            canBeTagged = false;
            StartCoroutine(WaitForTagAbility());
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        
    }

    private IEnumerator WaitForTagAbility()
    {
        yield return new WaitForSeconds(tagDelay);
        Debug.Log("Can tag again");
        canBeTagged = true;
    }
}
