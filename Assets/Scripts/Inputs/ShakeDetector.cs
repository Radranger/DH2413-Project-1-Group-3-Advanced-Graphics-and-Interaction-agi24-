using UnityEngine;
using System;

public class ShakeDetector
{
    public bool IsShaking { get; private set; }
    public event Action OnShakeStarted;
    public event Action OnShakeStopped;

    private Vector3 previousAcceleration;
    private float shakeThreshold;
    private float lowPassFilterFactor;

    public ShakeDetector(float shakeThreshold = 1.5f, float lowPassKernelWidthInSeconds = 1.0f, float updateFrequency = 60f)
    {
        this.shakeThreshold = shakeThreshold;
        lowPassFilterFactor = updateFrequency * lowPassKernelWidthInSeconds;
        previousAcceleration = Input.acceleration;
    }

    public void UpdateShakeDetection(Vector3 currentAcceleration)
    {
        Vector3 filteredAcceleration = Vector3.Lerp(previousAcceleration, currentAcceleration, lowPassFilterFactor);
        float shakeIntensity = (filteredAcceleration - previousAcceleration).magnitude;

        if (shakeIntensity > shakeThreshold && !IsShaking)
        {
            IsShaking = true;
            OnShakeStarted?.Invoke();
        }
        else if (shakeIntensity <= shakeThreshold && IsShaking)
        {
            IsShaking = false;
            OnShakeStopped?.Invoke();
        }

        previousAcceleration = filteredAcceleration;
    }
}
