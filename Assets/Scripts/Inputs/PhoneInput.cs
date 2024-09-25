using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhoneInput : IInputProvider
{
    private NetworkPlayer _networkPlayer;
    public event Action OnShoot;
    public PhoneInput(NetworkPlayer networkPlayer)
    {
        _networkPlayer = networkPlayer;

        _networkPlayer.OnPhoneFire1 += shoot;
    }

    public Vector2 GetMovementVector()
    {
        float inputX = _networkPlayer.GetX();
        float inputY = -(_networkPlayer.GetY());
        return new Vector2(inputX, inputY);
    }

    public void shoot()
    {
        OnShoot?.Invoke();
    }
}
