using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public GameObject FieldRespawn;

    public int height = 100;
    public float speed = 0.1f;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        time = height / speed;
        StartCoroutine(Spawner());
    }

    IEnumerator Spawner()
    {
        while(true)
        {
            GameObject asteroidField = Instantiate(FieldRespawn, new Vector3(0, 0, (3/2) * height), Quaternion.identity);
            AsteroidField field = asteroidField.GetComponent<AsteroidField>();
            field.fieldHeight = height;
            field.movementSpeed = speed;

            StartCoroutine(DestroyIfOutOfBounds(asteroidField));

            yield return new WaitForSeconds(time * 0.02f);
        }
    }

    IEnumerator DestroyIfOutOfBounds(GameObject asteroidField)
    {
        // Continuously check the position of the asteroid field
        while (asteroidField != null)
        {
            // Check if the z position of the asteroid field is less than -50f
            if (asteroidField.transform.position.z < -50f)
            {
                // Destroy the asteroid field if it's out of bounds
                Destroy(asteroidField);
                yield break;  // Exit the coroutine
            }

            // Wait for the next frame before checking again
            yield return null;
        }
    }
}
