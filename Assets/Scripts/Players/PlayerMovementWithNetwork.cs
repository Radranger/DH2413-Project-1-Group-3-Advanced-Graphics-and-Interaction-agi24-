using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementWithNetwork : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("The game object used to access accelerometer inputs from the phone.")]
    private NetworkPlayer m_networkPlayer;
    
    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-2,4); //Initial Position(Random Range)
    
    [SerializeField]
    private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();
    
    public Vector2 movementBoundaryX = new Vector2(-3f, 9f); // X-axis boundaries
    public Vector2 movementBoundaryY = new Vector2(-6f, 6f); // Y-axis boundaries
    
    private Vector3 oldInputPosition = Vector3.zero;
    private Vector2 InitialAccelerometerValue;
   
    private Vector2 _frameVelocity;
    private Vector2 _velocity;
    private Rigidbody _rb;
    private BoxCollider _col;
    private Transform spaceShipTransform;

    private float threshold = 0.25f;
    
    void Start()
    {
        if (IsClient && IsOwner) //generate player character in random position
        {
            //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
              //  Random.Range(defaultPositionRange.x, defaultPositionRange.y));
              

        }
        spaceShipTransform = transform.GetChild(0); // 获取第一个子对象
        _rb = spaceShipTransform.GetComponent<Rigidbody>();
        _col = spaceShipTransform.GetComponent<BoxCollider>();
        
        //Save the initial phone attitude
        InitialAccelerometerValue.x = m_networkPlayer.GetX();
        InitialAccelerometerValue.y = m_networkPlayer.GetY();

    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient && IsOwner)
        {

        }
        FrameReset();
        HandleDirection();
        HandleRotation();
        ApplyMovement();
    }
    
    void FrameReset(){
        _frameVelocity = new Vector3(0.0f,0.0f,0.0f);
    }
    
    void HandleDirection()
    {
        Vector2 input = CalculateMovementVector(InitialAccelerometerValue.x, InitialAccelerometerValue.y,
            m_networkPlayer.GetX(), m_networkPlayer.GetY(), threshold);

        input = Vector2.ClampMagnitude(input, 1f);

        Vector2 acceleration = input * GlobalSettings.ACCELERATION; 

        _velocity += acceleration * Time.deltaTime;

        if (Mathf.Approximately(input.x, 0f))
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, GlobalSettings.DECELERATION * Time.deltaTime); 
        }

        if (Mathf.Approximately(input.y, 0f))
        {
            _velocity.y = Mathf.MoveTowards(_velocity.y, 0, GlobalSettings.DECELERATION * Time.deltaTime); 
        }

        _velocity = Vector2.ClampMagnitude(_velocity, GlobalSettings.MAX_SPEED);

        _frameVelocity = _velocity * Time.deltaTime;
    }
    void HandleRotation(){
        Vector3 currentRotation = spaceShipTransform.eulerAngles;
        currentRotation.z = (_velocity.x / GlobalSettings.MAX_SPEED) * -30.0f;
        spaceShipTransform.eulerAngles = currentRotation;
    }
    
    // Calculates movement direction based on accelerometer input.
    // Returns -1, 0, or 1 for X and Y directions depending on whether the input exceeds the threshold.
    private Vector2 CalculateMovementVector(float iniX, float iniY, float inputX, float inputY, float threshold)
    {
        float x = Mathf.Abs(inputX - iniX) > threshold ? (inputX > iniX ? 1 : -1) : 0; 
        float y = Mathf.Abs(inputY - iniY) > threshold ? (inputY > iniY ? -1 : 1) : 0;

        return new Vector2(x, y);
    }

    //with boundary
    private void ApplyMovement()
    {
        _rb.velocity = _frameVelocity;
        Vector3 currentPosition = _rb.position;
        
        currentPosition.x = Mathf.Clamp(currentPosition.x, movementBoundaryX.x, movementBoundaryX.y);
        currentPosition.y = Mathf.Clamp(currentPosition.y, movementBoundaryY.x, movementBoundaryY.y);
        
        _rb.position = currentPosition;
    }

}
