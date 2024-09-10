using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSpace;

public class InputManager
{
    private IInputProvider _inputProvider;
   
    // Inits the input system,is debug will set it to keyboard input
    public void Initialize(InputType inputType){
        if(inputType == InputType.KEYBOARD){
            _inputProvider =  new DebugKeyboardInput();
        }
        if(inputType == InputType.PHONE){
            throw new System.Exception("Phone script not connected yet");
            _inputProvider =  null; // add phoneinput script here
        }
    }

    void start(){
        
    }
    void update(){

    }

    public Vector2 GetMovementVector(){
        return _inputProvider.GetMovementVector();
    }
}
