using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;

public class Player : MonoBehaviour
{
    private bool _active;
    private InputManager _inputManager;
    private PlayerMovement _playerMovement;

    public void Initialize(InputManager inputManager){
        _active = true;
        _inputManager = inputManager;
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.Initialize(_inputManager);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
