using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSpace;

public class PlayerMovementNEW : MonoBehaviour
{
    private InputManager _inputManager;
    private Vector2 _frameVelocity;
    private Vector2 _velocity;
    private Rigidbody _rb;
    private BoxCollider _col;

    public Vector2 maxTilt = new Vector2(0.5f, 0.5f);
    public float thresholdX = 0.2f;
    public float thresholdY = 0.2f;
    public float acceleration = 0.001f;
    public float maxSpeed = 0.2f;
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
    void FixedUpdate()
    {
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
        VisualDif.x = _inputManager.GetMovementVector().x;
        VisualDif.y = _inputManager.GetMovementVector().y;
    }
    
    void FrameReset(){
        _frameVelocity = new Vector2(0.0f,0.0f);
    }
    void HandleDirection()
    {
        Vector2 rawInput = _inputManager.GetMovementVector();
        
        Vector2 thresholdTargetVector = new Vector2(
            Mathf.Abs(rawInput.x) < thresholdX ? 0.0f : rawInput.x, 
            Mathf.Abs(rawInput.y) < thresholdY ? 0.0f : rawInput.y
        );
        
        Vector2 maxTiltApplied = new Vector2(Mathf.Clamp((thresholdTargetVector.x / maxTilt.x), -1.0f, 1.0f), Mathf.Clamp((thresholdTargetVector.y / maxTilt.y), -1.0f, 1.0f));
        //Debug.Log($"Movement Vector: X = {movementVector.x}, Y = {movementVector.y}");
        

        //float delta = 0.0f;
        
        _velocity = Vector2.MoveTowards(_velocity, thresholdTargetVector, acceleration);
        
        _frameVelocity = maxSpeed * _velocity;
    }
    void HandleRotation(){
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = (_velocity.x / GlobalSettings.MAX_SPEED) * -30.0f;
        transform.eulerAngles = currentRotation;
    }

    private void ApplyMovement()
    {
        //Debug.Log($"Applying Movement: _frameVelocity = {_frameVelocity}, _velocity = {_velocity}");

        _rb.velocity = new Vector3(_frameVelocity.x, _frameVelocity.y, 0.0f);


        // bounds check
        Vector3 currentPosition = transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -15f, 15f);
        currentPosition.y = Mathf.Clamp(currentPosition.y, -10f, 10f);

        transform.position = currentPosition;
    }
}
