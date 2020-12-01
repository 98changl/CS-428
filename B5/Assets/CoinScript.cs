using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(90 * Time.deltaTime,0 ,0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Agent tag -> Player or Daniel
        if (other.tag == "Daniel")
        {
            other.GetComponent<DisplayCoinsCollected>().coinsCollected++;
            Destroy(gameObject);
        }
    }
}
