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

    public Vector2 GetMovementVector(){
        return new Vector2(_networkPlayer.GetX(), _networkPlayer.GetY());
    }

    public int GetShooting()
    {
        return _networkPlayer.RequestShoot();
    }
}
