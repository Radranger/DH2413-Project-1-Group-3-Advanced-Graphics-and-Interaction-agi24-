using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;

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

    // Mapping player and its NetworkPlayer Object
    private Dictionary<ulong, Player> _playerDictionary = new Dictionary<ulong, Player>();

    public Vector2 playerMass; // The current average player position.

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _obstacleSpawnerObject = GameObject.Find("SpawnPlane");
        _obstacleManager = _obstacleSpawnerObject.GetComponent<ObstacleManager>();

        StartCoroutine(calcMass());
        
        if (DebugMode)
        {
            AddLocalPlayer();
        }
    }

    IEnumerator calcMass()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
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

    public void AddPlayer(NetworkPlayer networkPlayer)
    {
        InputManager _inputManager = new InputManager();
        _inputManager.Initialize(InputType.PHONE, networkPlayer);

        Vector3 spawnPos = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject playerObject = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);

        Player playerScript = playerObject.GetComponent<Player>();
        playerScript.Initialize(_inputManager, _playerPrefab);

        _playerDictionary.Add(networkPlayer.OwnerClientId, playerScript);
    }
    public void AddLocalPlayer(){
        InputManager _inputManager = new InputManager();
        _inputManager.Initialize(InputType.KEYBOARD);

        Vector3 spawnPos = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject playerObject = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);

        Player playerScript = playerObject.GetComponent<Player>();
        playerScript.Initialize(_inputManager, _playerPrefab);
        
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
    }
}
