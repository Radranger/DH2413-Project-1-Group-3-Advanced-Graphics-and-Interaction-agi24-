using UnityEngine;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool hasHit = false;
    private Player _source;

    void OnTriggerEnter(Collider collision)
    {
        // Check if the colliding object is an Obstacle
        if (!hasHit && collision.gameObject.CompareTag("enemy"))
        {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hasHit = true;
                obstacle.RegularHit(_source);

                // Destroy the bullet
                Destroy(gameObject);
            }
        }
    }

    public void setSource(Player src)
    {
        _source = src;
    }
}