using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public List<GameObject> pickupPrefabs;    // List of pickup item prefabs
    public float spawnInterval = 5f;          // Base spawn interval
    public float randomIntervalRange = 3f;    // Random interval range
    public float spawnRadius = 10f;           // Spawn radius


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
            float randomInterval = spawnInterval + Random.Range(-randomIntervalRange, randomIntervalRange);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPickup()
    {
        Vector3 spawnPosition = GetRandomPositionOnPlane();
        int randomIndex = Random.Range(0, pickupPrefabs.Count);
        GameObject randomPickup = pickupPrefabs[randomIndex];

        Instantiate(randomPickup, spawnPosition, Quaternion.identity);
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
