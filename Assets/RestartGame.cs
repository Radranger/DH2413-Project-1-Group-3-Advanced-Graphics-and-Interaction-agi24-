using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void OnRestartButtonClick()
    {
        if (ServerManager.Instance != null)
        {
            Debug.Log("Restarting server and switching to MAIN_SCENE.");
            //ServerManager.Instance.RestartServer();
        }
        else
        {
            Debug.LogError("ServerManager instance is not found!");
        }
    }
}
