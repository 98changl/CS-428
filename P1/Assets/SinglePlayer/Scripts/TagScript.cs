using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagScript : MonoBehaviour
{
    public bool isTagged;

    public int points;
    private float time;
    private int delay = 1;
    private bool canBeTagged;
    private float tagDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        points = 0;
        canBeTagged = false;
        StartCoroutine(WaitForTag());
    }

    // Update is called once per frame
    void Update()
    {
        if(isTagged)
        {
            // No points awarded
        }
        else
        {
            time += Time.deltaTime;
            if(time >= delay)
            {
                time = 0f;
                points++;
            }
            Debug.LogFormat("points = {0}", points);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if(other.gameObject.tag == "player" && canBeTagged)
        {
            Debug.Log("Player Trigger");
            if(isTagged == false)
            {
                isTagged = true;
            }
            else
            {
                isTagged = false;
            }
            canBeTagged = false;
            StartCoroutine(WaitForTag());
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if(collision.gameObject.tag == "player" && canBeTagged)
        {
            Debug.Log("Player Collision");
            if(isTagged == false)
            {
                isTagged = true;
            }
            else
            {
                isTagged = false;
            }
            canBeTagged = false;
            StartCoroutine(WaitForTag());
        }
    }

    public void OnCollisionExit(Collision collision)
    {

    }

    private IEnumerator WaitForTag()
    {
        yield return new WaitForSeconds(tagDelay);
        canBeTagged = true;
    }
}
