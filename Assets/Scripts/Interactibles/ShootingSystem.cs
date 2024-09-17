using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    public float bulletSpeed = 20.0f;
    public GameObject spaceship;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bullet.transform.position = spaceship.transform.position;
        //bullet.transform.rotation = spaceship.transform.rotation;

        // Add a rigidbody component so that the bullet is affected by physics
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        BoxCollider collider = bullet.GetComponent<BoxCollider>();
        collider.isTrigger = false;

        //bullet.AddComponent<Bullet>();

        rb.velocity = spaceship.transform.forward * bulletSpeed;
        Destroy(bullet, 5.0f);
    }
}
