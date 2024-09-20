using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;

public class Player : MonoBehaviour
{
    private bool _active;
    private InputManager _inputManager;
    private PlayerMovement _playerMovement;
    private ShootingSystem _shootingSystem;

    public void Initialize(InputManager inputManager, GameObject playerPrefab){
        _active = true;
        _inputManager = inputManager;
        //if(networkPlayer != null) _networkPlayer = networkPlayer;

        if (TryGetComponent(out _playerMovement)){_playerMovement.Initialize(_inputManager);};
        if (TryGetComponent(out _shootingSystem))
        {
            
        }
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
