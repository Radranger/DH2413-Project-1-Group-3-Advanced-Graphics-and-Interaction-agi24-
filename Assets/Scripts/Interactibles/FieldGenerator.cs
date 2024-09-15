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
            yield return new WaitForSeconds(time * 0.02f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
