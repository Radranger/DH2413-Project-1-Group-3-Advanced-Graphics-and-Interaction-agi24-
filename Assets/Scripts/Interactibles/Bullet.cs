using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is an Obstacle
        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            //obstacle.RegularHit();

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}