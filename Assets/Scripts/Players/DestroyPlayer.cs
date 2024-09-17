using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayer : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign Game Over UI panel

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
            // Destroy the player GameObject
            Destroy(gameObject);
            CheckAllPlayersDestroyed();
        }
    }

    // Check if all players are destroyed
    void CheckAllPlayersDestroyed()
    {
        if (gameObject == null && !isGameOver)
        {
            // Game is over
            Time.timeScale = 0;  // Pause the game
            isGameOver = true;    // Mark game as over
            Debug.Log("Game Over! All players destroyed.");

            // Optional: Show Game Over panel
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }
}
