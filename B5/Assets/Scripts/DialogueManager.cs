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

    private string EVIL_TEXT_NODE0 = "I am Dr Evil and I have rigged a binary bomb that will blow up this entire game unless my demands are met!";
    private string EVIL_TEXT_NODE2 = "All you have to do is steal the keys to the hacker's car and deliver them to me.";
    private string EVIL_TEXT_NODE3 = "HA! You think you can defuse my binary bomb? Good Luck.";
    private string EVIL_TEXT_NODE13 = "Nice, you got the keys. I have deactivated the binary bomb, good job.";

    private string HACKER_TEXT_NODE0 = "If you need something hacked I am your guy, it may cost you a pretty penny though.";
    private string HACKER_TEXT_NODE2 = "Just grab three coins around the map and deliver them to me, once you get them all I will hack the bomb and disable it.";
    private string HACKER_TEXT_NODE8 = "Ok fine, I will hack the bomb for free, just dont hurt me!";

    private string BUTTON1_NODE0 = "What does this button do? *Press Button";
    private string BUTTON2_NODE0 = "Tell me what your demands are and I will see what I can do.";
    private string BUTTON3_NODE0 = "I dont negotiate with terrorists, you will be stopped!";

    private string BUTTON1_NODE5 = "Pickpocket the hacker for his keys.";
    private string BUTTON2_NODE6 = "Fight the hacker for his car keys (50% Chance you lose the fight.)";
    private string BUTTON3_NODE2 = "Fight the hacker and force him to hack the bomb for free (50% Chance you lose the fight.)";

    //private string BUTTON1_NDOE1 = "";

    private void Start()
    {
        dialogueWindow.enabled = false;
        var mainTextBox = dialogueWindow.GetComponentInChildren<UnityEngine.UI.Text>();
        var button1 = dialogueWindow.GetComponentInChildren<UnityEngine.UI.Button>().GetComponentInChildren<UnityEngine.UI.Text>();
        var button2 = dialogueWindow.GetComponentInChildren<UnityEngine.UI.Button>().GetComponentInChildren<UnityEngine.UI.Text>();
        var button3 = dialogueWindow.GetComponentInChildren<UnityEngine.UI.Button>().GetComponentInChildren<UnityEngine.UI.Text>();

        mainTextBox.text = EVIL_TEXT_NODE0;

        button1.text = "Debugger";

        Debug.Log(mainTextBox);
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
