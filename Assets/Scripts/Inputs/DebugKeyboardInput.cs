using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DebugKeyboardInput : MonoBehaviour, IInputProvider
{
    public event Action OnShoot;
    public DebugKeyboardInput()
    {
    }

    public Vector2 GetMovementVector(){
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnShoot?.Invoke();
        }
    }
}
