using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotation : MonoBehaviour
{
    private Vector3 randomAxis;
    public float rotationSpeed = 10f; 

    void Start()
    {
        randomAxis = Random.onUnitSphere;  
    }

    void Update()
    {
        transform.Rotate(randomAxis, rotationSpeed * Time.deltaTime);
    }
}
