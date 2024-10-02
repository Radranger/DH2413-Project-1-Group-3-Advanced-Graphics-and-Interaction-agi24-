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
    
    //---------------------For debug--------------------------
    
    private void DestroyThisPlayer()
    {
        Debug.Log("DestroyPlayer: Player is being destroyed.");
        
        // Notify the ServerManager that this player is destroyed
        ServerManager.Instance.PlayerDestroyed(gameObject);

        // Play explosion sound effect
        if (shipExplodeClip != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(shipExplodeClip, transform, 1f);
        }

        // Destroy the player GameObject
        Destroy(gameObject);
    }
    
    void Update()
    {
        // Check if the 'K' key is pressed for manual destruction
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Debug Mode: Manual player destruction triggered.");
            DestroyThisPlayer();
        }
    }
}
