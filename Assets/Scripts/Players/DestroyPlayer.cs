using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayer : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign Game Over UI panel
    [SerializeField] private AudioClip shipExplodeClip; // Inspector audio clip

    private bool isGameOver; // Tracks game-over state

    void Start()
    {
        isGameOver = false;
    }

    // Check for trigger collisions
    void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with the asteroid
        if (other.gameObject.tag == "enemy")
        {
            Debug.Log("DestroyPlayer: Player collided with enemy. Requesting destruction.");
            ServerManager.Instance.PlayerDestroyed(gameObject);

            // Destroy the player GameObject
            Destroy(gameObject);


            SoundFXManager.instance.PlaySoundFXClip(shipExplodeClip, transform, 1f);
            Debug.Log("DestroyPlayer: Destroy(gameObject) called.");

            //CheckAllPlayersDestroyed();
        }
    }

    // Check if all players are destroyed
    // void CheckAllPlayersDestroyed()
    // {
    //     if (gameObject == null && !isGameOver)
    //     {
    //         // Game is over
    //         Time.timeScale = 0;  // Pause the game
    //         isGameOver = true;    // Mark game as over
    //         Debug.Log("Game Over! All players destroyed.");

    //         // Optional: Show Game Over panel
    //         if (gameOverPanel != null)
    //         {
    //             gameOverPanel.SetActive(true);
    //         }
    //     }
    // }
}
