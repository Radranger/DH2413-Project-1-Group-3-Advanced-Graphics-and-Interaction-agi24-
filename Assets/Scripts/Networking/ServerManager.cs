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
    private int countdownTime = 5;

    // For Debug
    [SerializeField]
    private TMP_Text playerInfoText;
    public List<GameObject> networkPlayers = new List<GameObject>();

    // A map from Player GameObject to NetworkPlayer GameObject
    private Dictionary<GameObject, GameObject> playerMap = new Dictionary<GameObject, GameObject>();

    // A map from player ID to Player GameObject
    private Dictionary<ulong, GameObject> playerIdMap = new Dictionary<ulong, GameObject>();

    // List of Player GameObjects
    public List<GameObject> players = new List<GameObject>();

    // Map with active players
    private Dictionary<GameObject, bool> activePlayers = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> activeNetworkPlayers = new Dictionary<GameObject, bool>();

    public bool gameStarted = false;

    private GameManager _gameManager;
    private GameObject _gameManagerObject;

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

        // Start updating player names
        StartCoroutine(SetPlayerNames());
        StartCoroutine(UpdatePlayerInfo());
    }

    private void OnServerStopped(bool obj)
    {
        Debug.LogWarning("Server stopped! Going back to title screen");

        // make sure to destroy the network manager before reload scene
        Destroy(gameObject);

        SceneManager.LoadScene("MAIN_SCENE");
    }

    // _____________________________________________________
    // Debug

    private IEnumerator UpdatePlayerInfo()
    {
        while (true)
        {
            string playerInfo = "";
            foreach (GameObject networkPlayer in networkPlayers)
            {
                NetworkPlayer networkPlayerComponent = networkPlayer.GetComponent<NetworkPlayer>();
                // get player's acceleration
                Vector3 accelerometer = networkPlayerComponent.accelerometer.Value;
                string playerName = networkPlayerComponent.GetPlayerName();

                // display
                playerInfo += $"{playerName}: X={accelerometer.x:F2}, Y={accelerometer.y:F2}, Z={accelerometer.z:F2}\n";
            }

            // update UI
            if (playerInfoText != null)
            {
                playerInfoText.text = playerInfo;
            }

            yield return new WaitForSeconds(1); // Updated once per second
        }
    }

    // _____________________________________________________

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
    }

    private void OnClientConnected(ulong clientID)
    {
        Debug.Log("Client connected: " + clientID);
        Debug.Log("Number of clients connected: " + NetworkManager.Singleton.ConnectedClientsList.Count);

        // find the NetworkPlayer object
        GameObject networkPlayer = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.gameObject;

        networkPlayers.Add(networkPlayer);

        // Instantiate the Player object through GameManager
        _gameManager.AddPlayer(networkPlayer.GetComponent<NetworkPlayer>());

        // Get the player GameObject from GameManager
        GameObject playerObject = _gameManager.GetPlayerGameObjectByClientId(clientID);

        if (playerObject != null)
        {
            // Keep track of the player
            players.Add(playerObject);
            playerMap.Add(playerObject, networkPlayer);
            playerIdMap.Add(clientID, playerObject);
        }
        else
        {
            Debug.LogError("Player object not found for client ID: " + clientID);
        }

        // Start updating player info
        StartCoroutine(UpdatePlayerInfo());
    }

    private void OnClientDisconnected(ulong clientID)
    {
        // Implement disconnection logic if necessary
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

    // _____________________________________________________
    // Update player names above their heads
    private IEnumerator SetPlayerNames()
    {
        while (true)
        {
            foreach (GameObject player in players)
            {
                GameObject networkPlayer = playerMap[player];
                NetworkPlayer networkPlayerComponent = networkPlayer.GetComponent<NetworkPlayer>();
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
        // Implement end game logic if necessary

        resetGameButton.gameObject.SetActive(false);

        activeNetworkPlayers.Clear();

        gameStarted = false;

        // Show menu UI
        startGameButton.gameObject.SetActive(true);
        menuScreen.SetActive(true);
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
