using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAreaManager : MonoBehaviour
{
    public Collider startArea;
    public GameObject countdownUI;
    private bool gameStarted = false;
    public float countdownTime = 3.0f; // time for ready
    
    private Dictionary<ulong, bool> playersInArea = new Dictionary<ulong, bool>();
    
    private Coroutine countdownCoroutine;
    
    //---------------------------for debug--------------------------
    public bool PlayerIn;
    public string PlayerClientId;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerIn = true;
        if (gameStarted) return;
        
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            NetworkPlayer networkPlayer = player.GetNetworkPlayer();
            if (networkPlayer != null)
            {
                playersInArea[networkPlayer.OwnerClientId] = true;
                ServerManager.Instance.SetPlayerReady(networkPlayer.OwnerClientId, true);
            }
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        PlayerIn = false;
        if (gameStarted) return; 

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            NetworkPlayer networkPlayer = player.GetNetworkPlayer();
            if (networkPlayer != null && playersInArea.ContainsKey(networkPlayer.OwnerClientId))
            {
                playersInArea[networkPlayer.OwnerClientId] = false;
                ServerManager.Instance.SetPlayerReady(networkPlayer.OwnerClientId, false);
            }
        }
    }
    
    
    //------------------------Countdown----------------------------
    
   
}
