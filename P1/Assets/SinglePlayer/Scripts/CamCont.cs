using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCont : MonoBehaviour
{
    GameObject player;
    Vector3 camOffset;
    public int x;
    public int y;
    public int z;

    private void Start()
    {
        player = GameObject.Find("Player");
        
    }

    private void Update()
    {
        camOffset = new Vector3(x, y, z);
        transform.position = player.transform.position + camOffset;
    }
}
