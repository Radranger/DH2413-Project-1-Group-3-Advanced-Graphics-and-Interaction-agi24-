using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;
using System;

public class InputManager
{
    private IInputProvider _inputProvider;
   
    // Inits the input system,is debug will set it to keyboard input
    public void Initialize(InputType inputType, NetworkPlayer networkPlayer = null){
        if(inputType == InputType.KEYBOARD){
            _inputProvider =  new DebugKeyboardInput();
        }
        if(inputType == InputType.PHONE){
            _ = networkPlayer ?? throw new ArgumentException("networkPlayer cannot be null when using phone"); 
            _inputProvider =  new PhoneInput(networkPlayer);
        }
    }

    void start(){
        
    }
    void update(){

    }

    public Vector2 GetMovementVector(){
        return _inputProvider.GetMovementVector();
    }

    /*public int GetShooting()
    {
        return _inputProvider.GetShooting();
    }*/
}
