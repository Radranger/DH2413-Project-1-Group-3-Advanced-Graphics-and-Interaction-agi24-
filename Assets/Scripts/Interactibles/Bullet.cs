using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        // Check if the colliding object is an Obstacle
        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
        if (obstacle != null && collision.gameObject.CompareTag("enemy"))
        {
            obstacle.RegularHit();

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}