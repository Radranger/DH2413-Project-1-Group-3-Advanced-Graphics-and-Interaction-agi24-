using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;
    public float appearDuration = 0.1f;
    public float existingTime = 5f;

    private Vector3 startPosition;
    private Vector3 targetScale;
    private float spawnTime;

    void Start()
    {
        startPosition = transform.position;
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        spawnTime = Time.time;

        Destroy(gameObject, existingTime);
    }

    void Update()
    {
        float elapsed = Time.time - spawnTime;

        if (elapsed < appearDuration)
        {
            float scaleProgress = Mathf.Clamp01(elapsed / appearDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, scaleProgress);
        }
        else if (elapsed > existingTime - appearDuration)
        {
            float scaleProgress = Mathf.Clamp01((existingTime - elapsed) / appearDuration);
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, scaleProgress);
        }
        else
        {
            transform.localScale = targetScale;
        }

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Destroy(gameObject);
        }
    }
}
