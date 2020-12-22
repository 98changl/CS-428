using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;

    public float timer = 120;
    public bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        isRunning = true;   
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                Debug.Log("Time is out");
                timer = 0;
                isRunning = false;
            }
        }  
    }

    void DisplayTime(float time)
    {
        float min = Mathf.FloorToInt(time / 60);
        float sec = Mathf.FloorToInt(time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }
}
