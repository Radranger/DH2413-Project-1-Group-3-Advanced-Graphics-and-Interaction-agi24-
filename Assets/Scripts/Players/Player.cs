using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;

public class Player : MonoBehaviour
{
    private bool _active;
    private InputManager _inputManager;
    private PlayerMovementNEW _playerMovementNEW;
    private ShootingSystem _shootingSystem;
    private NetworkPlayer _networkPlayer;
    private int _score;
    
    public int Score
    {
        get => _score;
        set => _score = value;
    }
    
    

    
    public ulong playerID;

    public void Initialize(InputManager inputManager, GameObject playerPrefab, ulong iplayerID, NetworkPlayer networkPlayer = null)
    {
        playerID = iplayerID;
        _active = true;
        _inputManager = inputManager;
        _networkPlayer = networkPlayer;
        //if(networkPlayer != null) _networkPlayer = networkPlayer;

        if (TryGetComponent(out _playerMovementNEW)){_playerMovementNEW.Initialize(_inputManager);};
        if (TryGetComponent(out _shootingSystem)){ _shootingSystem.Initialize(_inputManager); }
        // _playerMovement = GetComponent<PlayerMovement>();
        // _playerMovement.Initialize(_inputManager);   

    }
    
    public NetworkPlayer GetNetworkPlayer()
    {
        return _networkPlayer;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
