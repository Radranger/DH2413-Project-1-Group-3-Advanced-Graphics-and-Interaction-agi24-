using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlayer : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign Game Over UI panel
    [SerializeField] private AudioClip shipExplodeClip; // Inspector audio clip

    private bool isGameOver; // Tracks game-over state

    public int playerHealth;
    private int _currentHealth;
    
    private bool _canBeHit;

    void Start()
    {
        isGameOver = false;
        _currentHealth = playerHealth;
        _canBeHit = true;
    }

    // Check for trigger collisions
    void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with the asteroid
        if (other.gameObject.tag == "enemy")
        {
            if (_canBeHit)
            {
                StartCoroutine(coolDown());
                _canBeHit = false;
                SoundFXManager.instance.PlaySoundFXClip(shipExplodeClip, transform, 1f);
                if (_currentHealth == 1)
                {
                    Debug.Log("DestroyPlayer: Player collided with enemy. Requesting destruction.");
                    ServerManager.Instance.PlayerDestroyed(gameObject);

                    // Destroy the player GameObject
                    Destroy(gameObject);
                    
                    Debug.Log("DestroyPlayer: Destroy(gameObject) called.");
                }
                else
                {
                    
                    _currentHealth--;
                }
                
            }
            //CheckAllPlayersDestroyed();
        }
    }

    IEnumerator coolDown()
    {
        yield return new WaitForSeconds(1f);
        _canBeHit = true;
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
