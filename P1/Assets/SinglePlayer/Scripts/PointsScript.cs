using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsScript : MonoBehaviour
{
    public GameObject[] players;
    public Text playerScore;
    public Text AIScore;
    // Start is called before the first frame update
    void Start()
    {
            var initialPoints = 0;
            playerScore.text = initialPoints.ToString();
            AIScore.text = initialPoints.ToString();
            players = GameObject.FindGameObjectsWithTag("player");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(players.Length);
        foreach(GameObject player in players)
        {
            var TagScript = player.GetComponent<TagScript>();
            var points = TagScript.points;
            //Debug.Log(points.ToString());                        
            //Debug.Log("Got Here");

            if(player.name.Equals("Player"))
            {
                playerScore.text = points.ToString();
            }
            else
            {
                AIScore.text = points.ToString();
            }
        }
    }
}
