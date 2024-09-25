using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSpace;

public class PlayerMovement : MonoBehaviour
{
    private InputManager _inputManager;
    private Vector2 _frameVelocity;
    private Vector2 _velocity;
    private Rigidbody _rb;
    private BoxCollider _col;
    private Vector2 InitialAccelerometerValue = Vector2.zero;

    public float threshold = 0.2f;
    public Vector2 VisualInput;
    public Vector2 VisualDif;
    public Vector2 VisualInitialValue;
    

    public void Initialize(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<BoxCollider>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    // Update is called once per frame
    void Update()
    {
        //save the initial position
        if(_inputManager.playerInputType == InputType.PHONE){
            if(InitialAccelerometerValue == Vector2.zero){} InitialAccelerometerValue = _inputManager.GetMovementVector();
        }
        
        FrameReset();
        HandleDirection();
        HandleRotation();
        ApplyMovement();

        //Debug
        VisualValue();

        //visualInput = _inputManager.GetMovementVector();
    }

    void VisualValue()
    {
        VisualInitialValue = InitialAccelerometerValue;
        VisualInput = CalculateMovementVector(InitialAccelerometerValue, _inputManager.GetMovementVector(), threshold);
        VisualDif.x = InitialAccelerometerValue.x - (_inputManager.GetMovementVector().x);
        VisualDif.y = InitialAccelerometerValue.y - (_inputManager.GetMovementVector().y);
    }
    
    void FrameReset(){
        _frameVelocity = new Vector3(0.0f,0.0f,0.0f);
    }
    void HandleDirection()
    {
        Vector2 movementVector = _inputManager.GetMovementVector();

        Vector2 input = CalculateMovementVector(InitialAccelerometerValue, movementVector, threshold);

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
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = (_velocity.x / GlobalSettings.MAX_SPEED) * -30.0f;
        transform.eulerAngles = currentRotation;
    }
    
    private Vector2 CalculateMovementVector(Vector2 InitialPosition, Vector2 Input, float threshold)
    {
        float x = Mathf.Abs(Input.x - InitialPosition.x) > threshold ? (Input.x > InitialPosition.x ? 1 : -1) : 0;
        float y = Mathf.Abs(Input.y - InitialPosition.y) > threshold ? (Input.y > InitialPosition.y ? 1 : -1) : 0;

        return new Vector2(x, y);
    }

    private void ApplyMovement()
    {
        _rb.velocity = _frameVelocity;


        // bounds check
        Vector3 currentPosition = transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -15f, 15f);
        currentPosition.y = Mathf.Clamp(currentPosition.y, -10f, 10f);

        transform.position = currentPosition;
    }
}
