using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    public float respawnTime;
    public List<GameObject> PickUps;
    
    public void SpawnFunction()
    {
        for (int i = 0; i < PickUps.Count; i++)
        {
            // object is not respawning and not active
            if (PickUps[i].activeInHierarchy == false && PickUps[i].GetComponent<Flag>().Respawning == false)
            {
                PickUps[i].GetComponent<Flag>().IsRespawning();
                Respawn(PickUps[i]);
            }
        }
    }

    void Respawn(GameObject target)
    {
        StartCoroutine(WaitForSpawn(target));
    }

    public IEnumerator WaitForSpawn(GameObject target)
    {
        yield return new WaitForSeconds(respawnTime);
        target.gameObject.SetActive(true);
        target.GetComponent<Flag>().IsNotRespawning();
    }

}
