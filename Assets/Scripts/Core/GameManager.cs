using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameSpace;
using TMPro;

namespace GameSpace
{
    public enum InputType
    {
        KEYBOARD,
        PHONE
    }
}

public class GameManager : MonoBehaviour
{
    // [SerializeField] private InputType _inputType;
    // private InputManager _inputManager;

    // [SerializeField] private GameObject _player;
    // private Player _playerScript;

    public bool DebugMode = true;
    
    public static GameManager Instance { get; private set; }
    
    public Vector2 bounds = new Vector2(10, 20); // height and with of bounds total bounds is this * 2 from -x -> x

    [SerializeField] private GameObject _playerPrefab;
    private GameObject _obstacleSpawnerObject;
    private ObstacleManager _obstacleManager;

    private GameObject _pickupSpawnerObject;
    private PickupSpawner _pickupSpawner;
    // Mapping player and its NetworkPlayer Object
    private Dictionary<ulong, Player> _playerDictionary = new Dictionary<ulong, Player>();

    public Vector2 playerMass; // The current average player position.

    private GameObject _playerAmountUIElement;
    private TextMeshProUGUI _playerAmountUIElementTextMeshPro;
    public TextMeshProUGUI player1;
    public TextMeshProUGUI player2;
    public TextMeshProUGUI player3;
    public TextMeshProUGUI player4;

    private float _gameProgress;
    public Action onGameProgressChanged;

    public GameObject progressBar;
    private float _fullWidth;

    public int ScoreToFinish = 100;

    public GameObject gameUICanvas;
    public GameObject gameScoreboardUICanvas;
    
    
    public float GameProgress
    {
        get => _gameProgress;
        set
        {
            if (_gameProgress != value)
            {
                _gameProgress = value;
                onGameProgressChanged?.Invoke();
            }
        }
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
        
        onGameProgressChanged += GameProgressUpdate;
    }

    public void GameProgressUpdate()
    {
        Debug.Log($"Game Progress: {GameProgress}");
        
        foreach(Player player in _playerDictionary.Values){
            Debug.Log($"{player.playerID} Game Progress: {player.Score}");
        }
        RectTransform rect = progressBar.GetComponent<RectTransform>();
        Debug.Log("full width: " + _fullWidth);
        Debug.Log($"Game Progress pixels: {(_gameProgress / (float)ScoreToFinish) * _fullWidth}");
        rect.sizeDelta = new Vector2((_gameProgress / (float)ScoreToFinish) * _fullWidth, rect.sizeDelta.y);

        if (_gameProgress == ScoreToFinish) FinishGame();
    }

    private void Start()
    {
        _obstacleSpawnerObject = GameObject.Find("SpawnPlane");
        _obstacleManager = _obstacleSpawnerObject.GetComponent<ObstacleManager>();
        
         _playerAmountUIElement = GameObject.Find("PlayersAmount");
         _playerAmountUIElementTextMeshPro = _playerAmountUIElement.GetComponent<TextMeshProUGUI>();

         RectTransform rect = progressBar.GetComponent<RectTransform>();
         _fullWidth = rect.sizeDelta.x;
         rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
         gameUICanvas.SetActive(false);
         gameScoreboardUICanvas.SetActive(false);


        _pickupSpawnerObject = GameObject.Find("PickupSpawn");
        _pickupSpawner = _pickupSpawnerObject.GetComponent<PickupSpawner>();

        StartCoroutine(calcMass());
        
        if (DebugMode)
        {
            AddLocalPlayer();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
                Time.timeScale = 1;
                gameScoreboardUICanvas.SetActive(false);
        }
    }

    private void FinishGame()
    {
        TextMeshProUGUI[] playerTexts = { player1, player2, player3, player4 };
        var sortedPlayers = _playerDictionary.OrderByDescending(element => element.Value.Score).Select(element => element.Value).ToList();
        int index = 0;

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            string name = sortedPlayers[i].NetworkPlayer.GetPlayerName();
            int score = sortedPlayers[i].Score;
            playerTexts[i].text = $"{name}: {score}";
        }
        Time.timeScale = 0;

        gameScoreboardUICanvas.SetActive(true);
    }

    IEnumerator calcMass()
    
    {
        
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if(_playerDictionary.Count == 0){
                playerMass = Vector2.zero;
                continue;
            }
            playerMass = Vector2.zero;
            int length = _playerDictionary.Count;
            foreach (var player in _playerDictionary.Values)
            {
                playerMass.x += player.transform.position.x / bounds.x;
                playerMass.y += player.transform.position.y / bounds.y;
            }
            playerMass /= length;
        }
    }

    void updatePlayerLimit()
    {
        _playerAmountUIElementTextMeshPro.text = _playerDictionary.Count.ToString() + "/4 Connected";
    }

    public void AddPlayer(NetworkPlayer networkPlayer)
    {
        if (_playerDictionary.Count >= 4) return;
        
        
        InputManager _inputManager = new InputManager();
        _inputManager.Initialize(InputType.PHONE, networkPlayer);

        Vector3 spawnPos = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject playerObject = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);

        Player playerScript = playerObject.GetComponent<Player>();
        playerScript.Initialize(_inputManager, _playerPrefab, networkPlayer.OwnerClientId, networkPlayer);

        _playerDictionary.Add(networkPlayer.OwnerClientId, playerScript);

        updatePlayerLimit();
    }
    
    public void AddLocalPlayer(){
        InputManager _inputManager = new InputManager();
        _inputManager.Initialize(InputType.KEYBOARD);

        Vector3 spawnPos = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject playerObject = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);

        Player playerScript = playerObject.GetComponent<Player>();
        playerScript.Initialize(_inputManager, _playerPrefab, 100);
        
        _playerDictionary.Add(0, playerScript);
    }

    public GameObject GetPlayerGameObjectByClientId(ulong clientId)
    {
        if (_playerDictionary.TryGetValue(clientId, out Player player))
        {
            return player.gameObject;
        }
        return null;
    }

    public Player GetPlayerByClientId(ulong clientId)
    {
        _playerDictionary.TryGetValue(clientId, out Player player);
        return player;
    }

    public void RemovePlayer(ulong clientId)
    {
        if (_playerDictionary.TryGetValue(clientId, out Player player))
        {
            Destroy(player.gameObject);
            _playerDictionary.Remove(clientId);
        }
    }

    public void ClearPlayers()
    {
        foreach (KeyValuePair<ulong, Player> kvp in _playerDictionary)
        {
            Destroy(kvp.Value.gameObject);
        }
        _playerDictionary.Clear();
    }

    public void StartGame()
    {
        _obstacleManager.startSpawning();
        _pickupSpawner.StartSpawningPickup();
        gameUICanvas.SetActive(true);
    }

}
