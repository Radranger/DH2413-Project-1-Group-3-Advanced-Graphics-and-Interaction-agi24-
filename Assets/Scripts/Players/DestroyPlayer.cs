using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayer : MonoBehaviour
{
    public GameObject player; // Assign the player object in the Inspector
    public GameObject gameOverPanel; // Optional: Assign Game Over UI panel

    private bool isGameOver = false; // Tracks game-over state

    // Check for trigger collisions
    void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with the asteroid
        if (other.CompareTag("Player"))
        {
            // Destroy the player GameObject
            Destroy(other.gameObject);
            CheckAllPlayersDestroyed();
        }
    }

    // Check if all players are destroyed
    void CheckAllPlayersDestroyed()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null && !isGameOver)
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
