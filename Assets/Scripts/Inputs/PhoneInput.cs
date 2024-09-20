using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneInput : IInputProvider
{
    private NetworkPlayer _networkPlayer;
    public PhoneInput(NetworkPlayer networkPlayer)
    {
        _networkPlayer = networkPlayer;
    }

    public Vector2 GetMovementVector()
    {
        float inputX = _networkPlayer.GetX();
        float inputY = -(_networkPlayer.GetY());
        return new Vector2(inputX, inputY);
    }

    public int GetShooting()
    {
        return 0;
    }
}
