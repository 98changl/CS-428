using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerA;
    public GameObject playerB;

    private Vector3 offset;
    private Vector3 center;

    // Use this for initialization
    void Start()
    {
        center = ((playerA.transform.position + playerB.transform.position) / 2.0f);
        offset = transform.position - center;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        center = ((playerA.transform.position + playerB.transform.position) / 2.0f);
        transform.position = center + offset;
    }
}
