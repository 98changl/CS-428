using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public static int coins;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(90 * Time.deltaTime,0 ,90 * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // // Agent tag -> Player or Daniel
        if (other.tag == "Player")
        {
            Destroy(gameObject);
            coins++;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10,10,100,20), "Coins : " + coins);
    }
}
