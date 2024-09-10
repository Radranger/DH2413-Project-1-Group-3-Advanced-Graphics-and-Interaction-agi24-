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
    [SerializeField] private InputType _inputType;
    private InputManager _inputManager;

    [SerializeField] private GameObject _player;
    private Player _playerScript;

    void Start()
    {
        _inputManager = new InputManager();
        _inputManager.Initialize(_inputType);
        _playerScript = _player.GetComponent<Player>();
        _playerScript.Initialize(_inputManager);
    }
}