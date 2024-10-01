using UnityEngine;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool hasHit = false;

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger enter ** ");
        // Check if the colliding object is an Obstacle
        if (!hasHit && collision.gameObject.CompareTag("enemy"))
        {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hasHit = true;
                obstacle.RegularHit();

                // Destroy the bullet
                Destroy(gameObject);
            }
        }
    }
}