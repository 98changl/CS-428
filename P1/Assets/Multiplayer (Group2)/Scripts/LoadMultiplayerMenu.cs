using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMultiplayerMenu : MonoBehaviour
{
    public void MultiplayerMenu()
    {
        SceneManager.LoadScene(sceneBuildIndex:1);
    }
}
