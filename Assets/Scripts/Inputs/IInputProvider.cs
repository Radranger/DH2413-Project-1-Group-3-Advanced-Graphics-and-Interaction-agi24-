using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInputProvider
{
    public event Action OnShoot;
    Vector2 GetMovementVector();
}
