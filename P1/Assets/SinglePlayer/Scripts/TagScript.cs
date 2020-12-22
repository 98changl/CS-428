using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagScript : MonoBehaviour
{
    public bool isTagged;

    private int points;
    private float time;
    private int delay = 1;
    // Start is called before the first frame update
    void Start()
    {
        points = 0;
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
}
