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
    public float accelerationSpeed = 0.16f;
    public float maxSpeed = 0.2f;
    public Vector2 VisualInput;
    public Vector2 VisualDif;
    public Vector2 VisualInitialValue;
    
    //--- for shake detection ---
    public bool shakestatement;
    public float shakeThreshold = 2.0f; 
    private bool isShaking = false;
    private int shakeCount = 0;
    private float lastShakeTime = 0f;
    public float shakeDetectionWindow = 1f; 
    private float cooldownTimer = 0f;
    public float shakeCooldown = 0.5f; 
    //===
    
    private Vector2 _previousVelocity;
    public Vector2 _acceleration;
    
    private bool _isFrozen = false;
    private Vector3 _frozenPosition;
    private FreezePlayer _freezePlayerScript;
    
    public float wobbleAmount = 30.0f;
    

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
        _previousVelocity = new Vector2(0, 0);
        _acceleration = new Vector2(0, 0);
        _freezePlayerScript = GetComponent<FreezePlayer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isFrozen)
        {
            _rb.velocity = Vector3.zero;
            transform.position = _frozenPosition;
            CheckForShake();
            return;
        }
        
        FrameReset();
        HandleDirection();
        HandleRotation();
        CalcAcceleration();
        ApplyMovement();
        

        //Debug
        VisualValue();

        //visualInput = _inputManager.GetMovementVector();
    }
    
    void HandleShakeStarted()
    {
        Debug.Log("Shake started!");
        shakestatement = true;
        if (_isFrozen) UnfreezeMovement();
        if (_freezePlayerScript != null)
        {
            _freezePlayerScript.Unfreeze();
        }
    }

    void HandleShakeStopped()
    {
        Debug.Log("Shake stopped!");
        shakestatement = false;
    }
    
    void CheckForShake()
    {
        Vector3 acceleration = _inputManager.GetMovementVector();
        
        if (Mathf.Abs(acceleration.x) > shakeThreshold || Mathf.Abs(acceleration.y) > shakeThreshold)
        {
            float currentTime = Time.time;
            
            if (currentTime - lastShakeTime > shakeDetectionWindow)
            {
                shakeCount = 0;
            }

            lastShakeTime = currentTime;
            shakeCount++;
            
            if (shakeCount >= 8 && !isShaking)
            {
                isShaking = true;
                shakestatement = true;
                HandleShakeStarted();
            }
            
            cooldownTimer = 0f;
        }
        else
        {
            if (isShaking)
            {
                cooldownTimer += Time.deltaTime;
                if (cooldownTimer >= shakeCooldown)
                {
                    isShaking = false;
                    shakestatement = false;
                    HandleShakeStopped();
                    shakeCount = 0;
                    cooldownTimer = 0f;
                }
            }
        }
    }

    void CalcAcceleration()
    {
        _acceleration = _velocity - _previousVelocity;
        _previousVelocity = _velocity;
    }
    
    public void FreezeMovement()
    {
        _isFrozen = true;
        _frozenPosition = transform.position;
    }

    public void UnfreezeMovement()
    {
        _isFrozen = false;
        
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
        
        _velocity = Vector2.MoveTowards(_velocity, thresholdTargetVector, accelerationSpeed);
        
        _frameVelocity = maxSpeed * _velocity;
    }
    // void HandleRotation(){
    //     Vector3 currentRotation = transform.eulerAngles;
    //     currentRotation.z = (_acceleration.x * wobbleAmount) * -30.0f;
    //     transform.eulerAngles = currentRotation;
    // }
    private Vector3 targetRotation;
    void HandleRotation()
    {
        //float targetZRotation = (_acceleration.x * wobbleAmount) * -50.0f;
        //targetRotation = new Vector3(0, 0, targetZRotation);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), rotationSpeed * Time.deltaTime);
        
        Vector3 targetRotation = new Vector3(_velocity.y * -20.0f, 0, _velocity.x * -20.0f);
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, 10f * Time.deltaTime);
        
        // Vector3 currentRotation = transform.eulerAngles;
        // currentRotation.z = (_velocity.x) * -20.0f;
        // currentRotation.x = (_velocity.y) * -20.0f;
        // transform.eulerAngles = currentRotation;
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
