using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public bool Respawning = false;

    public void IsRespawning()
    {
        Respawning = true;
    }

    public void IsNotRespawning()
    {
        Respawning = false;
    }
}
