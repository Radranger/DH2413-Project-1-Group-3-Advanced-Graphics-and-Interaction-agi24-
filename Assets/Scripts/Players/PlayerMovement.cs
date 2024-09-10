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
        ApplyMovement();
    }
    void FrameReset(){
        _frameVelocity = new Vector3(0.0f,0.0f,0.0f);
    }
    void HandleDirection(){
        Vector2 input = _inputManager.GetMovementVector();
        // float inp_x = Mathf.abs(input.x);
        //float normalized_velocity_x = _velocity.x / GlobalSettings.MAX_SPEED;
        // float vel_x = Mathf.abs(normalized_velocity_x);
        // float delta = vel_x < inp_x ? ACCELARATION : DECELERATION;
        // float direction = Mathf.sign(input.x);
        // float stopThresh = 0.2f;
        // float dec = input.x * direction < (normalized_velocity_x * direction - stopThresh) ? GlobalSettings.DECELERATION * direction: 0.0f;


        input = Vector2.ClampMagnitude(input, 1f);

        Vector2 acceleration = input * GlobalSettings.ACCELARATION;

        _velocity += acceleration * Time.deltaTime;

        // Apply deceleration when no input
        if (Mathf.Approximately(input.x, 0f))
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, GlobalSettings.DECELERATION * Time.deltaTime);
        }
        if (Mathf.Approximately(input.y, 0f))
        {
            _velocity.y = Mathf.MoveTowards(_velocity.y, 0, GlobalSettings.DECELERATION * Time.deltaTime);
        }

        // Clamp velocity to max speed
        _velocity = Vector2.ClampMagnitude(_velocity, GlobalSettings.MAX_SPEED);


        // _velocity.x =  Mathf.Min(100.0f, Mathf.Max(-100.0f, _velocity.x + input.x));
        // _velocity.y = Mathf.Min(100.0f, Mathf.Max(-100.0f, _velocity.y + input.y));

        _frameVelocity = _velocity * Time.deltaTime;
    }

    private void ApplyMovement() => _rb.velocity = _frameVelocity;
}
