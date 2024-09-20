using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputProvider
{
    Vector2 GetMovementVector();
    int GetShooting();
}
