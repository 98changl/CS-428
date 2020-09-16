using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public GameObject menuUI;
    public Text timer;
    public float timeLeft = 0.0f;

    void Start()
    {
        menuUI.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeLeft -= Time.deltaTime;
        TimerDisplay();

        if (timeLeft <= 0)
        {
            GameOver();
        }
    }

    void TimerDisplay()
    {
        int t = (int) timeLeft;
        timer.text = t.ToString() + " secs remaining";
    }

    private void GameOver()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
