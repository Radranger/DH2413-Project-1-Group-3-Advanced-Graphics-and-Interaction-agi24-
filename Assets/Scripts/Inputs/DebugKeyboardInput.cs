using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKeyboardInput : IInputProvider
{
    public Vector2 GetMovementVector(){
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
