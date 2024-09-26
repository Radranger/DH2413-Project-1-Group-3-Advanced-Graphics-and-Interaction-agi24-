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

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _obstacleSpawner;

    // Mapping player and its NetworkPlayer Object
    private Dictionary<ulong, Player> _playerDictionary = new Dictionary<ulong, Player>();

    private void Awake()
    {
        Instance = this;
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
        _obstacleSpawner.SetActive(true);
    }
}
