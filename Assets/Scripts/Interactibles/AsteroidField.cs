using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{

    // Placement of asteroids in the "field"
    public Transform asteroidPrefab;  // Prefab for asteroids
    public int fieldRadius = 1;  // Radius of the cylinder
    public int fieldHeight = 1000;  // Height of the cylinder
    public int asteroidCount = 100;  // Number of asteroids

    // Variables for movement
    private float rotationSpeed;
    public float minRotationSpeed = 1f;
    public float maxRotationSpeed = 5f;
    public float minMovement = 0.1f;
    public float maxMovement = 0.5f;

    void Start()
    {
        populateField();
        addPhysics();
    }

    void populateField()
    {
        // Loop to instantiate asteroids
        for (int loop = 0; loop < asteroidCount; loop++)
        {
            // Generate random position in cylindrical coordinates
            float angle = Random.Range(0f, Mathf.PI * 2);
            //float radius = Random.Range(0f, fieldRadius);  // Change only if we want to make it less cylindrical
            float height = Random.Range(-fieldHeight / 2f, fieldHeight / 2f);  // Random height in the cylinder

            // Convert cylindrical coordinates to Cartesian coordinates
            float xPos = fieldRadius * Mathf.Cos(angle);  // X coordinate
            float zPos = height;  // Z coordinate
            float yPos = fieldRadius * Mathf.Sin(angle);  // Y coordinate (height)

            // Get a random asteroid from the lowPolyAsteroids prefab within the asteroidField game object
            int randomAsteroidIndex = Random.Range(0, asteroidPrefab.GetChild(0).childCount);
            Transform randomAsteroid = asteroidPrefab.GetChild(0).GetChild(randomAsteroidIndex);

            // Instantiate asteroid at calculated position with random rotation
            Transform asteroid = Instantiate(randomAsteroid, new Vector3(xPos, yPos, zPos + 50f), Random.rotation);
            asteroid.localScale = asteroid.localScale * Random.Range(0.5f, 5);
        }
    }

    void addPhysics()
    {
            // Set random rotation speed
            rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

            // Add initial movement force
            float movement = Random.Range(minMovement, maxMovement);

            // apply physics to asteroid
            Rigidbody asteroidRb = GetComponent<Rigidbody>();
            asteroidRb.AddForce(transform.forward * movement, ForceMode.Impulse);
    }

    void Update()
    {

        // Rotate the asteroid field
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}