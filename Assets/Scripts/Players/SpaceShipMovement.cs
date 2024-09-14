using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The game object used to access accelerometer inputs from the phone.")]
    private NetworkPlayer m_networkPlayer;
    
    [SerializeField]
    [Range(0f, 0.5f)]
    [Tooltip("The threshold value for detecting left and right movement from the accelerometer input. Adjust to change sensitivity.")]
    private float m_LeftRigntDetectionThreshold = 0.3f;
    
    [SerializeField]
    [Range(0f, 0.5f)]
    [Tooltip("The threshold value for detecting up and down movement from the accelerometer input. Adjust to change sensitivity.")]
    private float m_UpDownDetectionThreshold = 0.3f;
    
    [SerializeField]
    [Range(0f, 3000f)]
    [Tooltip("The maximum speed that the player can reach while moving. Adjust this value to change the upper limit of movement speed.")]
    private float MAX_SPEED = 800f;

    [SerializeField]
    [Range(0f, 5000f)]
    [Tooltip("The rate at which the player's speed increases when moving. Adjust this value to control how quickly the player accelerates.")]
    private float ACCELERATION = 3500f;

    [SerializeField]
    [Range(0f, 5000f)]
    [Tooltip("The rate at which the player's speed decreases when movement input is reduced. Adjust this value to control how quickly the player slows down.")]
    private float DECELERATION = 4000f;

    
    
    [SerializeField]
    private Vector2 movementBoundaryX = new Vector2(-3f, 9f); // X-axis boundaries
    
    [SerializeField]
    public Vector2 movementBoundaryY = new Vector2(-6f, 6f); // Y-axis boundaries
    
    [SerializeField] private Vector2 TransformedInput;
    
    private Rigidbody m_rigidBody;
    private Transform spaceShipTransform;
    private Vector2 InitialAccelerometerValue;
    
    private Vector2 _frameVelocity;
    private Vector2 _velocity;
    
    // _____________________________________________________
    // Script lifecycle
    private void Start()
    {
        spaceShipTransform = transform.GetChild(0);
        m_rigidBody = spaceShipTransform.GetComponent<Rigidbody>();
        
        //Save the initial phone attitude
        InitialAccelerometerValue.x = m_networkPlayer.GetX();
        InitialAccelerometerValue.y = m_networkPlayer.GetY();
    }
    
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (m_networkPlayer == null)
        {
            return;
        }
        
        //Get Phone Sensor Data and Transform
        float inputX = m_networkPlayer.GetX();
        float inputY = m_networkPlayer.GetY();
        TransformedInput = CalculateMovementVector(InitialAccelerometerValue.x, InitialAccelerometerValue.y,
            inputX, inputY, m_LeftRigntDetectionThreshold, m_UpDownDetectionThreshold);
        
        FrameReset();
        HandleDirection(TransformedInput);
        HandleRotation();
        ApplyMovement();
    }
    
    // _____________________________________________________
    // 
    
    void FrameReset(){
        _frameVelocity = new Vector3(0.0f,0.0f,0.0f);
    }
    
    void HandleDirection(Vector2 input)
    {
        input = Vector2.ClampMagnitude(input, 1f);

        Vector2 acceleration = input * ACCELERATION; 

        _velocity += acceleration * Time.deltaTime;

        if (Mathf.Approximately(input.x, 0f))
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, DECELERATION * Time.deltaTime); 
        }

        if (Mathf.Approximately(input.y, 0f))
        {
            _velocity.y = Mathf.MoveTowards(_velocity.y, 0, DECELERATION * Time.deltaTime); 
        }

        _velocity = Vector2.ClampMagnitude(_velocity, MAX_SPEED);

        _frameVelocity = _velocity * Time.deltaTime;
    }
    
    void HandleRotation(){
        Vector3 currentRotation = spaceShipTransform.eulerAngles;
        currentRotation.z = (_velocity.x / MAX_SPEED) * -30.0f;
        spaceShipTransform.eulerAngles = currentRotation;
    }
    
    private Vector2 CalculateMovementVector(float iniX, float iniY, float inputX, float inputY, 
        float leftRightThreshold, float upDownThreshold)
    {
        float x = Mathf.Abs(inputX - iniX) > leftRightThreshold ? (inputX > iniX ? 1 : -1) : 0; 
        float y = Mathf.Abs(inputY - iniY) > upDownThreshold ? (inputY > iniY ? -1 : 1) : 0;

        return new Vector2(x, y);
    }
    
    private void ApplyMovement()
    {
        m_rigidBody.velocity = _frameVelocity;
        Vector3 currentPosition = m_rigidBody.position;
        
        currentPosition.x = Mathf.Clamp(currentPosition.x, movementBoundaryX.x, movementBoundaryX.y);
        currentPosition.y = Mathf.Clamp(currentPosition.y, movementBoundaryY.x, movementBoundaryY.y);
        
        m_rigidBody.position = currentPosition;
    }
    
    public void SetNetworkPlayer(NetworkPlayer networkPlayer)
    {
        m_networkPlayer = networkPlayer;
    }
}
