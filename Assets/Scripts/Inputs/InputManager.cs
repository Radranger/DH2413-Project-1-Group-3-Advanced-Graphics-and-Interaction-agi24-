using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;
using System;
using UnityEngine.Events;

public class InputManager
{
    private IInputProvider _inputProvider;

    public event Action OnShoot;

    public InputType playerInputType;

   
    // Inits the input system, is debug will set it to keyboard input
    public void Initialize(InputType inputType, NetworkPlayer networkPlayer = null){
        playerInputType = inputType;
        if(inputType == InputType.KEYBOARD){
            GameObject inputGameObject = new GameObject("KeyboardInputHandler");
            _inputProvider = inputGameObject.AddComponent<DebugKeyboardInput>();
        }
        if(inputType == InputType.PHONE){
            _ = networkPlayer ?? throw new ArgumentException("networkPlayer cannot be null when using phone"); 
            _inputProvider =  new PhoneInput(networkPlayer);
        }

        if (_inputProvider != null)
        {
            _inputProvider.OnShoot += HandleOnShoot;
        }
    }

    private void HandleOnShoot()
    {
        Debug.Log("Shootin");
        OnShoot?.Invoke();
    }

    public Vector2 GetMovementVector(){
        return _inputProvider.GetMovementVector();
    }
}
