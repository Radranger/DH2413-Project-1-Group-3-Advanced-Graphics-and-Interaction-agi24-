using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<Vector3> accelerometer = new NetworkVariable<Vector3>(
        Vector3.zero,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );
    
    [HideInInspector]
    public NetworkVariable<bool> shootInput = new NetworkVariable<bool>(
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );

    [HideInInspector]
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(
        string.Empty,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );

    [HideInInspector]
    public NetworkVariable<Color> skinColor = new NetworkVariable<Color>(
        Color.black,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    private Vector3 prevAccelerometerInput;

    [Header("Low Pass filter Settings")]

    [Tooltip("How many times per second to update the accelerometer.")]
    public float accelerometerUpdateFrequency = 100.0f;

    [Tooltip("The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).")]
    public float lowPassKernelWidthInSeconds = 1.0f;

    private float lowPassFilterFactor;

    public event Action OnPhoneFire1;

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            Logger.Instance.LogInfo("NetworkPlayer despawned on client");
            Debug.Log("NetworkPlayer despawned on client");

            // this is only called on the webgl client from the NetworkedPlayer
            // notify ui manager that we have despawned
            ClientUIManager.Instance.OnNetworkDespawn();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Logger.Instance.LogInfo("NetworkPlayer spawned on client");
        }


        if (IsOwner)
        {
            Logger.Instance.LogInfo("I am the owner");
            ClientUIManager.Instance.localNetworkPlayer = this;
            Debug.Log($"Assigned localNetworkPlayer. OwnerClientId = {OwnerClientId}, IsOwner = {IsOwner}");
            
            accelerometer.Value = Vector3.zero;

            playerName.Value = ClientUIManager.Instance.nameInputField.text;
           skinColor.OnValueChanged += (prevValue, newValue) =>
            {
                ClientUIManager.Instance.backgroundColor.color = newValue;
            };

            if (Accelerometer.current == null)
            {
                Logger.Instance.LogInfo("Accelerometer not found, make sure you are runnig on a mobile device");
            }
            else
            {
                SetupAccelerometer();
            }
            
            

        }

        // if (IsServer)
        // {
        //     accelerometer.OnValueChanged += OnAccelerometerChanged;
        // }
    }

    public override void OnDestroy()
    {
        if (IsServer)
        {
            accelerometer.OnValueChanged -= OnAccelerometerChanged;
        }
    }

    public void OnAccelerometerChanged(Vector3 prevValue, Vector3 newValue)
    {
        Debug.Log("Accelerometer changed from " + prevValue + " to " + newValue);
    }

    void SetupAccelerometer()
    {
        InputSystem.EnableDevice(Accelerometer.current);
        prevAccelerometerInput = Accelerometer.current.acceleration.ReadValue();
        lowPassFilterFactor = accelerometerUpdateFrequency * lowPassKernelWidthInSeconds;
    }

    void Update()
    {
        if (IsOwner)
        {
            Vector3 accelerometerInput = Accelerometer.current.acceleration.ReadValue();
            accelerometerInput = LowPassFilterAccelerometer(prevAccelerometerInput, accelerometerInput);
            accelerometer.Value = accelerometerInput;
            prevAccelerometerInput = accelerometerInput;
            Logger.Instance.LogInfo("Accelerometer: " + accelerometer.Value);
        }
        if (IsServer)
        {
            // Debug.Log("Accelerometer: " + accelerometer.Value);
        }
    }

    Vector3 LowPassFilterAccelerometer(Vector3 prevValue, Vector3 newValue)
    {
        return Vector3.Lerp(prevValue, newValue, lowPassFilterFactor);
    }

    // public void SetPlayerName(string name)
    // {
    //     playerName.Value = name;
    // }

    public float GetX()
    {
        return accelerometer.Value.x;
    }

    public float GetY()
    {
        return accelerometer.Value.y;
    }

    public float GetZ()
    {
        return accelerometer.Value.z;
    }

    public string GetPlayerName()
    {
        return playerName.Value.ToString();
    }

    public void RequestShoot()
    {
        Debug.Log($"NetworkPlayer: RequestShoot() called. IsOwner = {IsOwner}, OwnerClientId = {OwnerClientId}, LocalClientId = {NetworkManager.Singleton.LocalClientId}");
        if (IsOwner)
        {
            Debug.Log("NetworkPlayer: RequestShoot() called on client");
            ShootServerRpc();
        }
        else
        {
            Debug.LogWarning("NetworkPlayer: RequestShoot() called but not owner");
        }
    }
    
    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("NetworkPlayer: ShootServerRpc() called on server");
        HandleShoot();
    }

    private void HandleShoot()
    {
        // 使用 GameManager 获取对应的 Player 对象
        Player player = GameManager.Instance.GetPlayerByClientId(OwnerClientId);
        if (player != null)
        {
            ShootingSystem shootingSystem = player.GetComponent<ShootingSystem>();
            if (shootingSystem != null)
            {
                //shootingSystem.Shoot();
                OnPhoneFire1?.Invoke();
            }
            else
            {
                Debug.LogWarning("ShootingSystem component not found on Player");
            }
        }
        else
        {
            Debug.LogWarning("Player object not found on server for this NetworkPlayer");
        }
    }
}

