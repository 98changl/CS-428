using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tagging : MonoBehaviour
{
    public Material agentIsNotIt;
    public Material agentIsIt;
    public Renderer agentColor;
    public GameObject self;

    public bool tagged;
    public float score;

    private bool canBeTagged;
    private float tagDelay = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        tagged = false;
        canBeTagged = true;
        //self.parent = GameObject.Find("PlayerManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && canBeTagged == true)
        {
            if (tagged == false)
            {
                tagged = true;
                agentColor.material = agentIsIt;
            }
            else
            {
                tagged = false;
                agentColor.material = agentIsNotIt;
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
        
    }

    public void OnCollisionExit(Collision collision)
    {
        
    }

    public IEnumerator WaitForTagAbility()
    {
        yield return new WaitForSeconds(tagDelay);
        canBeTagged = true;
    }
}
