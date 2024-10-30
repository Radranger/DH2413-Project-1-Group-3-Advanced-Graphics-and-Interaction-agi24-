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

    private bool _isInvincible; //check if the player got the shield
    private Renderer[] _playerRenderers;

    MeshRenderer _lightsRenderer;
    Color _playerColor;

    void Start()
    {
        isGameOver = false;
        _currentHealth = playerHealth;
        _canBeHit = true;
        _isInvincible = false;

        _playerRenderers = GetComponentsInChildren<Renderer>();
    }

    // Check for trigger collisions
    void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with the asteroid
        if (other.gameObject.tag == "enemy")
        {
            if (_canBeHit && !_isInvincible)
            {
                hit();
                
                
            }
            //CheckAllPlayersDestroyed();
        }
        else if (other.gameObject.tag == "shield")
        {
            StartCoroutine(Invincibility());
            Destroy(other.gameObject);
        }
    }

    void hit(){
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

        updateLights();
    }

    void updateLights(){
        
        GameObject shipHullObject = gameObject.transform.Find("ship")?.gameObject;
        
        if (shipHullObject == null)
        {
            Debug.LogWarning("shipHullObject or duckObject is null");
            return;
        }

        _lightsRenderer = shipHullObject.GetComponent<MeshRenderer>();
        Material[] materials = _lightsRenderer.materials;
        _playerColor = materials[2].color;

        
        if(_currentHealth == 1){StartCoroutine(criticalHealth());}
        if(_currentHealth == 2){StartCoroutine(healthLowButNotCritical());}
        if(_currentHealth == 3){}//light back to normal
    }

    IEnumerator criticalHealth()
    {
        Material[] materials = _lightsRenderer.materials;
        while(_currentHealth == 1){

            materials[2].color = Color.black;
            _lightsRenderer.materials = materials;

            yield return new WaitForSeconds(0.3f);

            
            materials[2].color = _playerColor;
            _lightsRenderer.materials = materials;

            yield return new WaitForSeconds(0.3f);

            
        }
    }
    IEnumerator healthLowButNotCritical()
    {
        Material[] materials = _lightsRenderer.materials;
        while(_currentHealth == 2){
        
            materials[2].color = Color.black;
            _lightsRenderer.materials = materials;

            yield return new WaitForSeconds(0.1f);

            materials[2].color = _playerColor;
            _lightsRenderer.materials = materials;

            yield return new WaitForSeconds(0.4f);
        }
        

    }

    IEnumerator coolDown()
    {
        yield return new WaitForSeconds(1f);
        _canBeHit = true;
    }
    IEnumerator Invincibility()
    {
        _isInvincible = true;
        float elapsedTime = 0f;
        float invincibilityDuration = 3f;
        float blinkInterval = 0.1f;

        Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
        foreach (Renderer renderer in _playerRenderers)
        {
            originalColors[renderer] = renderer.material.color;
        }

        while (elapsedTime < invincibilityDuration)
        {
            foreach (Renderer renderer in _playerRenderers)
            {
                Color color = renderer.material.color;
                color.a = (color.a == 1f) ? 0.3f : 1f;
                renderer.material.color = color;
            }

            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval;
        }

        foreach (Renderer renderer in _playerRenderers)
        {
            renderer.material.color = originalColors[renderer];
        }

        _isInvincible = false;
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
