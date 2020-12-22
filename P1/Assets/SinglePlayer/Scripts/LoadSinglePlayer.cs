using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSinglePlayer : MonoBehaviour
{
    public void Singleplayer()
    {
        SceneManager.LoadScene(sceneBuildIndex:3);
    }
}
