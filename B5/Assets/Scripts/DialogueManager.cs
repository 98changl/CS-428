using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DialogueManager : MonoBehaviour
{
    public GameObject mainCharacter;
    public GameObject drEvil;
    public GameObject hacker;

    public Canvas dialogueWindow;

    private void Start()
    {
        dialogueWindow.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToEvil = (mainCharacter.transform.position - drEvil.transform.position).magnitude;
        float distanceToHacker = (mainCharacter.transform.position - hacker.transform.position).magnitude;

        if(distanceToEvil <= 2.5f || distanceToHacker <= 2.5f)
        {
            dialogueWindow.enabled = true;
        }
        else
        {
            dialogueWindow.enabled = false;
        }
        
    }

}
