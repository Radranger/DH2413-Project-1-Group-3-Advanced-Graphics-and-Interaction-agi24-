using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject pickupPrefab; 
    public float spawnInterval = 5f;
    public float spawnRadius = 10f;

    private void Start()
    {
    }
    public void StartSpawningPickup()
    {
        StartCoroutine(SpawnPickupRoutine());
    }

    IEnumerator SpawnPickupRoutine()
    {
        while (true)
        {
            SpawnPickup();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPickup()
    {
        Vector3 spawnPosition = GetRandomPositionOnPlane();
        Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomPositionOnPlane()
    {

        // Assume the character moves on the X-Y plane with Z=0
        float x = Random.Range(-15f, 15f);
        float y = Random.Range(-10f, 10f);
        float z = 0f;

        return new Vector3(x, y, z);
    }
}
