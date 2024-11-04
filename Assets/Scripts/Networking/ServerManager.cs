using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
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
    private Dictionary<ulong, bool> playersReadyStatus = new Dictionary<ulong, bool>();

    public bool gameStarted = false;

    private GameManager _gameManager;
    private GameObject _gameManagerObject;
    private Coroutine playerNameCoroutine;
    
    private CancellationTokenSource cts;
    private GameObject _startGameArea;
    private GameObject _BackgroundAsteroids;
    public GameObject _QRimage;
    public GameObject _gamecode;
    private Coroutine countdownCoroutine;
    public float countdownTime = 10.0f;
    public TextMeshProUGUI countdownText; 
    public GameObject gameScoreboardUICanvas;
    private float finalCountdownTimer = 4.0f;
    public GameObject gameUICanvas;

    
    // ---------------------------------- Debug ----------------------------------


    // ---------------------------------- Unity Lifecycle ----------------------------------
    
    
    private async void Start()
    {
        cts = new CancellationTokenSource();
        
        if (NetworkManager.Singleton == null)
        {
            Debug.Log("NetworkManager is null, initializing a new one.");

        }
        else
        {
            Debug.LogWarning("NetworkManager already exists: " + NetworkManager.Singleton.gameObject.name);
        }

        // make sure only one NetworkManager 
        await InitializeServer(cts.Token);
    }

    void OnDisable()
    {
        Debug.Log("ServerManager::OnDisable Instance ID: " + this.GetInstanceID());
        StopServerAndDestroyNetworkManager();
    }
    
    private void OnDestroy()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartGame();
        }
        if (gameStarted)
        {
            CullPlayers();
        }
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
                if(gameStarted) EndGame();
                if(NetworkManager.Singleton.gameObject) Destroy(NetworkManager.Singleton.gameObject);
                if(_gameManager.gameObject) Destroy(_gameManager.gameObject);
                SceneManager.LoadScene("Scenes/RestartPagePC");
        }
    }
    
    // ---------------------------------- Initialization ----------------------------------
    
    private async Task InitializeServer(CancellationToken token) {
        Debug.Log("Initializing server.");
        _startGameArea = GameObject.Find("StartGameArea");
        _gameManagerObject = GameObject.FindWithTag("GameManager");
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        _BackgroundAsteroids = GameObject.Find("BackgroundAsteroids").gameObject;
        gameScoreboardUICanvas.SetActive(false);
        //gameUICanvas.SetActive(false);

        _gameManager.GameProgress = 0;
        _gameManager.GameProgressUpdate();

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

        // Configure Relay
        RelayHostData hostData;
        if (RelayManager.Instance.IsRelayEnabled)
        {
            try
            {
                hostData = await RelayManager.Instance.SetupRelay();
                // Check if cancellation is requested
                if (token.IsCancellationRequested) return;
                Debug.Log("Relay setup complete.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Relay setup failed: {ex.Message}");
                return;
            }
        }
        else
        {
            Debug.LogError("Relay is not enabled!");
            return;
        }

        // Start the server
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server started successfully!");
        }
        else
        {
            Debug.Log("Server failed to start!");
            return;
        }

        // Set join code
        if (joinCode != null)
        {
            TMP_Text tmpText = joinCode.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = hostData.JoinCode;
            }
        }
        else
        {
            Debug.LogWarning("joinCode object is null after async operation.");
        }

        // Register server callbacks
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
        // Check if cancellation is requested before starting coroutine
        if (token.IsCancellationRequested) return;
        playerNameCoroutine = StartCoroutine(SetPlayerNames());
    }
    
    // ---------------------------------- Client Management ----------------------------------
    
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
                //Debug.Log($"Assigned color {playerColor} to player {clientID}");

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
            if (np.GetComponent<NetworkObject>().OwnerClientId == clientID)
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
            playersReadyStatus.Remove(clientID);
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
    
    // ---------------------------------- Game State Management ----------------------------------
    
    private void StartGame()
    {
        gameStarted = true;
        // Hide menu UI
        startGameButton.gameObject.SetActive(false);
        menuScreen.SetActive(false);
        _BackgroundAsteroids.GetComponent<Animator>().enabled = true;


        _gameManager.StartGame();
    }
    
    public void EndGame()
    {
        activeNetworkPlayers.Clear();
        activePlayers.Clear();

        gameStarted = false;

        NetworkManager.Singleton.Shutdown();

        // 清理所有玩家
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
        playersReadyStatus.Clear();

        if (playerNameCoroutine != null)
        {
            StopCoroutine(playerNameCoroutine);
            playerNameCoroutine = null;
        }

        // clear networkPlayers
        foreach (GameObject networkPlayer in networkPlayers)
        {
            if (networkPlayer != null)
            {
                Destroy(networkPlayer);
            }
        }
        networkPlayers.Clear();
        activeNetworkPlayers.Clear();
        
        _gameManager.ClearPlayers();

        StartCoroutine(finalCountDown());
        
        if(NetworkManager.Singleton.gameObject) Destroy(NetworkManager.Singleton.gameObject);
        if(_gameManager.gameObject) Destroy(_gameManager.gameObject);
    }

    private IEnumerator finalCountDown()
    {
        finalCountdownTimer -= Time.deltaTime;
        if (finalCountdownTimer >= -1) countdownText.text = Mathf.Ceil(finalCountdownTimer).ToString();
        if (finalCountdownTimer <= 0) SceneManager.LoadScene("Scenes/RestartPagePC");
        
        yield return new WaitForSeconds(1);
    }

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
    
    public void SetPlayerReady(ulong clientId, bool isReady)
    {
        playersReadyStatus[clientId] = isReady;
        Debug.Log("Player " + clientId + " is " + (isReady ? "ready" : "not ready"));

        CheckAllPlayersReady();
    }
    
    private void CheckAllPlayersReady()
    {
        if (gameStarted) return;

        foreach (bool isReady in playersReadyStatus.Values)
        {
            if (!isReady)
            {
                StopCountdown();
                Debug.Log("Not all players are ready.");
                return;
            }
        }
        
        Debug.Log("All players are ready.");
        if (countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(StartCountdown());
            countdownText.gameObject.SetActive(true);
        }
    }
    
    private IEnumerator StartCountdown()
    {
        float timeRemaining = countdownTime;
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);  
        }
        _QRimage.gameObject.SetActive(false);
        _gamecode.gameObject.SetActive(false);

        while (timeRemaining > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(timeRemaining).ToString();  
            }
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);  
        }
        
        StartGame();
        _startGameArea.gameObject.SetActive(false);
    }
    
    private void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);  
            countdownCoroutine = null;

            if (countdownText != null)
            {
                countdownText.text = "";  
                countdownText.gameObject.SetActive(false); 
                _QRimage.gameObject.SetActive(true);
                _gamecode.gameObject.SetActive(true);
            }

            Debug.Log("Countdown stopped and reset.");
        }
    }
    
    // ---------------------------------- Server Shutdown and Cleanup ----------------------------------
    
    private void OnServerStopped(bool obj)
    {
        Debug.LogWarning("Server stopped! Going back to RestartPagePC");

        // Unsubscribe from events
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
        players.Clear();
        playerMap.Clear();
        playerIdMap.Clear();
        activePlayers.Clear();

        // 清理networkPlayers
        foreach (GameObject networkPlayer in networkPlayers)
        {
            Destroy(networkPlayer);
        }
        networkPlayers.Clear();

        // 清理GameManager中的玩家
        _gameManager.ClearPlayers();

        // 销毁 NetworkManager，防止重复创建
        if(NetworkManager.Singleton.gameObject) Destroy(NetworkManager.Singleton.gameObject);
        if(_gameManager.gameObject) Destroy(_gameManager.gameObject);

        // 加载RestartPagePC场景
        SceneManager.LoadScene("Scenes/RestartPagePC");
    }
    
    private void StopServerAndDestroyNetworkManager()
    {
        // Stop Server
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Server stopped.");
        }

        // Destroy NetworkManager Instance
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
            Debug.Log("NetworkManager destroyed.");
        }

        // make sure NetworkManager is destroyed
        if (NetworkManager.Singleton == null)
        {
            Debug.Log("NetworkManager is null after destruction.");
        }
        else
        {
            Debug.LogWarning("NetworkManager still exists after destruction.");
        }
    }



    // Debug: Update player info text




    /*public async void RestartServer()
    {
        //NetworkManager.Singleton.Shutdown();
        //menuScreen.SetActive(true);
        //Destroy(NetworkManager.Singleton.gameObject);
        // Manually invoke OnServerStopped since Shutdown doesn't trigger it automatically
        //OnServerStopped(true);
        
        SceneManager.LoadScene("Scenes/MAIN_SCENE");
        Debug.Log("Restarting Server successfully!");
    }
    private async Task RestartServerCoroutine()
    {
        await Task.Delay(100); // 等待100毫秒，等价于原来的WaitForSeconds(0.1f)
        InitializeSceneObjects();
        // 重新初始化服务器
        await InitializeServer(cts.Token);
    }
    private void InitializeSceneObjects()
    {
        // 重新查找并赋值 GameManager 的依赖项
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager not found after scene reload.");
        }
        else
        {
            // 如果有需要，还可以在这里重新分配其他依赖对象
            _gameManager.InitializeDependencies(); // 如果 GameManager 需要重新分配依赖项
        }
    }*/
    
    





    // Update player names above their heads



    private void CullPlayers()
    {
        // Implement culling logic if necessary
    }

    /// <summary>
    /// Ends the gameplay session, and returns to the "main menu".
    /// </summary>
   

    public void PlayerDestroyed(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            Debug.Log("Player removed. Remaining players: " + players.Count + gameStarted);
        }

        if (playerMap.ContainsKey(player))
        {
            GameObject networkPlayer = playerMap[player];
            playerMap.Remove(player);

            if (networkPlayer != null)
            {
                ulong clientID = networkPlayer.GetComponent<NetworkPlayer>().OwnerClientId;
                _gameManager.RemovePlayer(clientID);
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
            //ShowRestartScreen();
            EndGame();
            
            SceneManager.LoadScene("Scenes/RestartPagePC");
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
