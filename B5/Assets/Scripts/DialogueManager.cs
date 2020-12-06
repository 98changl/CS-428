using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public GameObject mainCharacter;
    public GameObject drEvil;
    public GameObject hacker;

    public BehaviorTree Arc;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;

    public Canvas dialogueWindow;
    public Canvas dialogueWindowHacker;
    public Canvas gameOverScreen;
    public Canvas gameOverScreenWin;

    private string EVIL_TEXT_NODE0 = "I am Dr Evil and I have rigged a binary bomb that will blow up this entire game unless my demands are met!";
    private string EVIL_TEXT_NODE1 = "No no no, dont press that but- \n\n *Bomb explodes, you lose.";
    private string EVIL_TEXT_NODE2 = "All you have to do is steal the keys to the hacker's car and deliver them to me.";
    private string EVIL_TEXT_NODE3 = "HA! You think you can defuse my binary bomb? Good Luck.";
    private string EVIL_TEXT_NODE4 = "In that case I will just detonate the bomb right now! \n\n *Bomb explodes, you lose";
    private string EVIL_TEXT_NODE5 = "You got the keys! Nice, I will steal his car later when he isnt looking. You are a hard worker, the bomb is now deactivated.";
    private string EVIL_TEXT_NODE7 = "What!?! Impossible! How did you defuse my binary bomb?";
    private string EVIL_TEXT_NODE9 = "It seems that the hacker beat you up real bad. Its a shame, without those car keys this bomb doesn't get deactivated. You lose!";
    private string EVIL_TEXT_NODE11 = "Ha ha ha! It looks like your plan didnt work as well as you thought it would. You lose.";
    private string EVIL_TEXT_NODE13 = "Nice, you got the keys. I have deactivated the binary bomb, good job.";

    private string HACKER_TEXT_NODE0 = "If you need something hacked I am your guy, it may cost you a pretty penny though.";
    private string HACKER_TEXT_NODE2 = "If you need something hacked I am your guy. It may cost you a pretty penny though, I got an expensive car and need to pay it off.";
    private string HACKER_TEXT_NODE3 = "I heard you needed a binary bomb defused. I can do it, but you need to collect 3 coins around the map first. I dont work for free.";
    private string HACKER_TEXT_NODE5 = "Huh, where did my car keys go? They must have fallen out of my pocket. Strange, I will look for them later.";
    private string HACKER_TEXT_NODE7 = "You got my pay, nice. I have just hacked the bomb and deactivated it. You no longer have to worry about Dr.Evil";
    private string HACKER_TEXT_NODE8 = "Ok fine, I will hack the bomb for free, just dont hurt me!";
    private string HACKER_TEXT_NODE9 = "Wow, it was surprisingly easy to beat you up, were you even trying to fight?";
    private string HACKER_TEXT_NODE10 = "Ouch! Ouch! Here take my keys, just stop hitting me!";
    private string HACKER_TEXT_NODE11 = "*Fight Lost! \n\n Maybe you should train more before going around fighting people!";
    private string HACKER_TEXT_NODE12 = "Ouch! Ouch! Fine, I'll hack the binary bomb for free. There it should be done, please dont hurt me!";

    private string BUTTON1_NODE0 = "What does this button do? *Press Button";
    private string BUTTON2_NODE0 = "Tell me what your demands are and I will see what I can do.";
    private string BUTTON3_NODE0 = "I dont negotiate with terrorists, you will be stopped!";

    private string BUTTON1_NODE2 = "I wont steal from another person. Count me out.";

    private string BUTTON4_NODE2 = "Pickpocket the hacker for his keys.";
    private string BUTTON5_NODE2 = "Fight the hacker for his car keys (50% Chance you lose the fight.)";

    private string BUTTON4_NODE3 = "Deliver 3 coins.";
    private string BUTTON5_NODE3 = "Fight the hacker and force him to hack the bomb for free (50% Chance you lose the fight.)";



    private void Start()
    {
        dialogueWindow.enabled = false;
        dialogueWindowHacker.enabled = false;
        gameOverScreen.enabled = false;
        gameOverScreenWin.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        float distanceToEvil = (mainCharacter.transform.position - drEvil.transform.position).magnitude;
        float distanceToHacker = (mainCharacter.transform.position - hacker.transform.position).magnitude;

        EvilPrinter();
        HackerPrinter();
        gameOver();
        gameOverWin();

        if (distanceToEvil <= 2.5f)
        {
            dialogueWindow.enabled = true;
        }
        else if (distanceToHacker <= 2.5f)
        {
            dialogueWindowHacker.enabled = true;
        }
        else
        {
            dialogueWindow.enabled = false;
            dialogueWindowHacker.enabled = false;
        }
    }

    void gameOverWin()
    {
        if (Arc.ArcGetter() == 5 || Arc.ArcGetter() == 13 || Arc.ArcGetter() == 12 || Arc.ArcGetter() == 7)
        {
            gameOverScreenWin.enabled = true;
        }
    }

    void gameOver()
    {
        if(Arc.ArcGetter() == 1 || Arc.ArcGetter() == 4 || Arc.ArcGetter() == 9 || Arc.ArcGetter() == 11)
        {
            gameOverScreen.enabled = true;
        }
    }

    void EvilPrinter()
    {
        var mainTextBox = dialogueWindow.GetComponentInChildren<UnityEngine.UI.Text>();

        if (Arc.ArcGetter() == 0)
        {
            mainTextBox.text = EVIL_TEXT_NODE0;
            button1.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON1_NODE0;
            button2.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON2_NODE0;
            button3.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON3_NODE0;
        }
        else if (Arc.ArcGetter() == 1)
        {
            mainTextBox.text = EVIL_TEXT_NODE1;
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else if(Arc.ArcGetter() == 2)
        {
            mainTextBox.text = EVIL_TEXT_NODE2;
            button1.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON1_NODE2;
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 3)
        {
            mainTextBox.text = EVIL_TEXT_NODE3;
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 4)
        {
            mainTextBox.text = EVIL_TEXT_NODE4;
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 5 || Arc.ArcGetter() == 10)
        {
            mainTextBox.text = EVIL_TEXT_NODE5;
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 7 || Arc.ArcGetter() == 12)
        {
            mainTextBox.text = EVIL_TEXT_NODE7;
        }
        else if (Arc.ArcGetter() == 9)
        {
            mainTextBox.text = EVIL_TEXT_NODE9;
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 11)
        {
            mainTextBox.text = EVIL_TEXT_NODE11;
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }


    }

    void HackerPrinter()
    {
        var mainTextBoxHacker = dialogueWindowHacker.GetComponentInChildren<UnityEngine.UI.Text>();

        if (Arc.ArcGetter() == 0)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE0;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
            button6.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 2)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE2;
            button4.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON4_NODE2;
            button5.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON5_NODE2;

            button4.gameObject.SetActive(true);
            button5.gameObject.SetActive(true);
        }
        else if (Arc.ArcGetter() == 3)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE3;
            button4.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON4_NODE3;
            button5.GetComponentInChildren<UnityEngine.UI.Text>().text = BUTTON5_NODE3;
            button4.gameObject.SetActive(true);
            button5.gameObject.SetActive(true);
        }
        else if (Arc.ArcGetter() == 5)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE5;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
            button6.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 7)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE7;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 9)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE9;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
            button6.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 10)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE10;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
            button6.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 11)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE11;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
            button6.gameObject.SetActive(false);
        }
        else if (Arc.ArcGetter() == 12)
        {
            mainTextBoxHacker.text = HACKER_TEXT_NODE12;
            button4.gameObject.SetActive(false);
            button5.gameObject.SetActive(false);
            button6.gameObject.SetActive(false);
        }



    }


}
