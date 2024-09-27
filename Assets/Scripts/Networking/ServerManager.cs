using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ServerManager : Singleton<ServerManager>
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private ColorPool skinPresets;

    [SerializeField]
    private GameObject menuScreen;

    [SerializeField]
    private GameObject joinCode;

    [SerializeField]
    private Button startGameButton;

    [SerializeField]
    private Button resetGameButton;

    [SerializeField]
    private UICountdown countdown;

    [SerializeField]
    private GameObject RestartScreen;

    [SerializeField]
    private Button RestartGameButton;

    [SerializeField]
    private int countdownTime = 5;

    // For Debug
    [SerializeField]
    private TMP_Text playerInfoText;

    // Lists and Dictionaries for managing players
    public List<GameObject> networkPlayers = new List<GameObject>();
    private Dictionary<GameObject, GameObject> playerMap = new Dictionary<GameObject, GameObject>();
    private Dictionary<ulong, GameObject> playerIdMap = new Dictionary<ulong, GameObject>();
    public List<GameObject> players = new List<GameObject>();
    private Dictionary<GameObject, bool> activePlayers = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> activeNetworkPlayers = new Dictionary<GameObject, bool>();

    public bool gameStarted = false;

    private GameManager _gameManager;
    private GameObject _gameManagerObject;
    private Coroutine playerNameCoroutine;

    private async void Start()
    {
        _gameManagerObject = GameObject.FindWithTag("GameManager");
        _gameManager = _gameManagerObject.GetComponent<GameManager>();

        countdown.ResetCountdown();

        // START SERVER
        startGameButton?.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 0)
            {
                return;
            }
            Debug.Log("Starting Game");
            StartGame();
        });

        resetGameButton?.onClick.AddListener(() =>
        {
            EndGame();
        });
        resetGameButton.gameObject.SetActive(false);

        // 
        //if (RestartGameButton != null)
        //{
        //    RestartGameButton.onClick.AddListener(() =>
        //    {
        //        Debug.Log("RestartGameButton clicked. Restarting server.");
        //        RestartServer();
        //        RestartScreen.SetActive(false);
        //    });
        //}
        //else
        //{
        //    Debug.LogError("RestartGameButton is not assigned in the Inspector.");
        //}

        //RestartScreen.SetActive(false);

        RelayHostData hostData;
        if (RelayManager.Instance.IsRelayEnabled)
        {
            hostData = await RelayManager.Instance.SetupRelay();
        }
        else
        {
            throw new Exception("Relay could not be enabled!");
        }

        if (NetworkManager.Singleton.StartServer())
            Debug.Log("Server started successfully!");
        else
            Debug.Log("Server failed to start!");

        joinCode.GetComponentInChildren<TMP_Text>().text = hostData.JoinCode;

        // Handle client connection and disconnection
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        }
        else
        {
            Debug.Log("Something went wrong! This is not a server!");
        }

        if (skinPresets != null)
        {
            // Reset the skin color preset pool
            skinPresets.ShuffleColors();
            skinPresets.ResetPool();
        }

        // Start updating player names and info
        playerNameCoroutine = StartCoroutine(SetPlayerNames());
        //StartCoroutine(UpdatePlayerInfo());
    }

    private void OnServerStopped(bool obj)
    {
        Debug.LogWarning("Server stopped! Going back to main scene");

        // Unsubscribe from events
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnServerStopped -= OnServerStopped;

        // Clean up all players
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
        players.Clear();
        playerMap.Clear();
        playerIdMap.Clear();
        activePlayers.Clear();

        // Clean up networkPlayers
        foreach (GameObject networkPlayer in networkPlayers)
        {
            Destroy(networkPlayer);
        }
        networkPlayers.Clear();

        // Clean up GameManager's player dictionary
        _gameManager.ClearPlayers();

        // Destroy the NetworkManager to avoid duplicates on scene reload
        Destroy(NetworkManager.Singleton.gameObject);

        // Load the main scene
        SceneManager.LoadScene("MAIN_SCENE");
    }

    // Debug: Update player info text
    private IEnumerator UpdatePlayerInfo()
    {
        while (true)
        {
            string playerInfo = "";
            for (int i = networkPlayers.Count - 1; i >= 0; i--)
            {
                GameObject networkPlayer = networkPlayers[i];
                if (networkPlayer == null)
                {
                    networkPlayers.RemoveAt(i);
                    continue;
                }

                NetworkPlayer networkPlayerComponent = networkPlayer.GetComponent<NetworkPlayer>();
                if (networkPlayerComponent == null)
                {
                    continue;
                }

                // Get player's acceleration
                Vector3 accelerometer = networkPlayerComponent.accelerometer.Value;
                string playerName = networkPlayerComponent.GetPlayerName();

                // Display
                playerInfo += $"{playerName}: X={accelerometer.x:F2}, Y={accelerometer.y:F2}, Z={accelerometer.z:F2}\n";
            }

            // Update UI
            if (playerInfoText != null)
            {
                playerInfoText.text = playerInfo;
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Start Game");
            StartGame();
        }
        if (gameStarted)
        {
            CullPlayers();
        }
        



        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameStarted)
            {
                EndGame();
                RestartServer();
            }
            else
            {
                RestartServer();
            }
        }
    }

    private void RestartServer()
    {
        NetworkManager.Singleton.Shutdown();

        // Manually invoke OnServerStopped since Shutdown doesn't trigger it automatically
        OnServerStopped(true);
    }

    private void OnClientConnected(ulong clientID)
    {
        Debug.Log("Client connected: " + clientID);
        Debug.Log("Number of clients connected: " + NetworkManager.Singleton.ConnectedClientsList.Count);

        // get NetworkPlayer object
        GameObject networkPlayer = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.gameObject;
        networkPlayers.Add(networkPlayer);

        _gameManager.AddPlayer(networkPlayer.GetComponent<NetworkPlayer>());

        GameObject playerObject = _gameManager.GetPlayerGameObjectByClientId(clientID);

        if (playerObject != null)
        {
            // Adding Player to the list and dictionary
            players.Add(playerObject);
            playerMap.Add(playerObject, networkPlayer);
            playerIdMap.Add(clientID, playerObject);

            networkPlayer.GetComponent<NetworkPlayer>().playerObject = playerObject;

            // assign skinColor
            if (skinPresets != null)
            {
                Color playerColor = skinPresets.PullColor();
                Debug.Log($"Assigned color {playerColor} to player {clientID}");

                networkPlayer.GetComponent<NetworkPlayer>().skinColor.Value = playerColor;
            }
            else
            {
                Debug.LogWarning("skinPresets is null");
            }
        }
        else
        {
            Debug.LogError("Player object not found for client ID: " + clientID);
        }

        StartCoroutine(UpdatePlayerInfo());
    }

    private void OnClientDisconnected(ulong clientID)
    {
        Debug.Log("Client disconnected: " + clientID);

        // Remove from networkPlayers
        GameObject networkPlayer = null;
        foreach (GameObject np in networkPlayers)
        {
            if (np.GetComponent<NetworkPlayer>().OwnerClientId == clientID)
            {
                networkPlayer = np;
                break;
            }
        }

        if (networkPlayer != null)
        {
            networkPlayers.Remove(networkPlayer);
            Destroy(networkPlayer);
        }
        else
        {
            Debug.LogWarning("NetworkPlayer not found for client ID: " + clientID);
        }

        // Remove from playerIdMap and playerMap
        if (playerIdMap.TryGetValue(clientID, out GameObject playerObject))
        {
            players.Remove(playerObject);
            playerMap.Remove(playerObject);
            playerIdMap.Remove(clientID);
            Destroy(playerObject);

            // Remove from activePlayers
            if (activePlayers.ContainsKey(playerObject))
            {
                activePlayers.Remove(playerObject);
            }
        }
        else
        {
            Debug.LogWarning("Player object not found for client ID: " + clientID);
        }

        // Remove player from GameManager
        _gameManager.RemovePlayer(clientID);

        // Check if all players have disconnected
        if (players.Count == 0 && gameStarted)
        {
            resetGameButton.gameObject.SetActive(true);
        }
    }


    private void StartGame()
    {
        Debug.Log("starting");
        // Hide menu UI
        startGameButton.gameObject.SetActive(false);
        menuScreen.SetActive(false);

        _gameManager.StartGame();

        gameStarted = true;
    }

    // Update player names above their heads
    private IEnumerator SetPlayerNames()
    {
        while (true)
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                GameObject player = players[i];

                if (player == null)
                {
                    // Remove from players list
                    players.RemoveAt(i);

                    // Remove from playerMap
                    if (playerMap.ContainsKey(player))
                    {
                        playerMap.Remove(player);
                    }

                    // Remove from activePlayers
                    if (activePlayers.ContainsKey(player))
                    {
                        activePlayers.Remove(player);
                    }

                    continue;
                }

                if (playerMap.ContainsKey(player))
                {
                    GameObject networkPlayer = playerMap[player];

                    if (networkPlayer == null)
                    {
                        // Remove from playerMap
                        playerMap.Remove(player);
                        continue;
                    }

                    NetworkPlayer networkPlayerComponent = networkPlayer.GetComponent<NetworkPlayer>();

                    if (networkPlayerComponent == null)
                    {
                        continue;
                    }

                    string playerName = networkPlayerComponent.GetPlayerName();

                    // Find the TMP_Text component on the player
                    TMP_Text text = player.GetComponentInChildren<TMP_Text>();
                    if (text != null)
                    {
                        text.text = playerName;
                    }
                    else
                    {
                        Debug.LogWarning("TMP_Text component not found on player: " + player.name);
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }
    }


    private void CullPlayers()
    {
        // Implement culling logic if necessary
    }

    /// <summary>
    /// Ends the gameplay session, and returns to the "main menu".
    /// </summary>
    public void EndGame()
    {
        resetGameButton.gameObject.SetActive(false);

        activeNetworkPlayers.Clear();
        activePlayers.Clear();

        gameStarted = false;

        // Show menu UI
        menuScreen.SetActive(true);

        // Clean up all players
        foreach (GameObject player in players)
        {
            if (player != null)
            {
                Destroy(player);
            }
        }
        players.Clear();
        playerMap.Clear();
        playerIdMap.Clear();
        activePlayers.Clear();

        if (playerNameCoroutine != null)
        {
            StopCoroutine(playerNameCoroutine);
            playerNameCoroutine = null;
        }
        
        // Clean up networkPlayers
        foreach (GameObject networkPlayer in networkPlayers)
        {
            if (networkPlayer != null)
            {
                Destroy(networkPlayer);
            }
        }
        networkPlayers.Clear();
        activeNetworkPlayers.Clear();

        // Clean up GameManager's player dictionary
        _gameManager.ClearPlayers();
    }

    public void PlayerDestroyed(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }

        if (playerMap.ContainsKey(player))
        {
            GameObject networkPlayer = playerMap[player];
            playerMap.Remove(player);

            if (networkPlayer != null)
            {
                networkPlayers.Remove(networkPlayer);
                Destroy(networkPlayer);
            }
        }

        // Remove from activePlayers
        if (activePlayers.ContainsKey(player))
        {
            activePlayers.Remove(player);
            Debug.Log("Remove from activePlayers");
        }

        // If all players are destroyed, end the game
        if (players.Count == 0 && gameStarted)
        {
            Debug.Log("All players have been destroyed. Activating RestartScreen.");
            RestartScreen.SetActive(true);
            EndGame();
        }
    }


    public int GetActivePlayers()
    {
        int count = 0;

        foreach (KeyValuePair<GameObject, bool> player in activeNetworkPlayers)
        {
            if (player.Value)
            {
                count++;
            }
        }

        return count;
    }
}
