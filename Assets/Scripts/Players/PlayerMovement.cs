using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    private InputManager _inputManager;
    private Vector2 _frameVelocity;
    private Vector2 _velocity;
    private Rigidbody _rb;
    private BoxCollider _col;
    
    public Vector2 visualInput;


    public void Initialize(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<BoxCollider>();
        //_rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        FrameReset();
        HandleDirection();
        HandleRotation();
        ApplyMovement();
        
        visualInput = _inputManager.GetMovementVector();
    }
    void FrameReset(){
        _frameVelocity = new Vector3(0.0f,0.0f,0.0f);
    }
    void HandleDirection()
    {
        Vector2 input = _inputManager.GetMovementVector();

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

    private void ApplyMovement() => _rb.velocity = _frameVelocity;
}
