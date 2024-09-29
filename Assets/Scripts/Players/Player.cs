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

    public void Initialize(InputManager inputManager, GameObject playerPrefab){
        _active = true;
        _inputManager = inputManager;
        //if(networkPlayer != null) _networkPlayer = networkPlayer;

        if (TryGetComponent(out _playerMovementNEW)){_playerMovementNEW.Initialize(_inputManager);};
        if (TryGetComponent(out _shootingSystem)){ _shootingSystem.Initialize(_inputManager); }
        // _playerMovement = GetComponent<PlayerMovement>();
        // _playerMovement.Initialize(_inputManager);   

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
