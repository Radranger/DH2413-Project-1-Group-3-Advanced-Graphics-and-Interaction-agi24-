using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FreezePlayer : MonoBehaviour
{
    public float freezeDuration = 3f;
    public GameObject freezeEffectPrefab;
    private bool _isFrozen;
    private Renderer[] _playerRenderers;
    private PlayerMovementNEW _playerMovementScript;
    private Animator _shakeTextAnimator;
    public GameObject _shakeText;
    private Rigidbody _rb;
    private RigidbodyConstraints _originalConstraints;

    private Color originalColor = Color.white;

    void Start()
    {
        _isFrozen = false;
        _playerRenderers = GetComponentsInChildren<Renderer>();
        _playerMovementScript = GetComponent<PlayerMovementNEW>();
        _shakeTextAnimator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _originalConstraints = _rb.constraints;

        if (_playerRenderers.Length > 0)
        {
            if (_playerRenderers[0].material.HasProperty("_Color"))
            {
                originalColor = _playerRenderers[0].material.color;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("frozen") && !_isFrozen)
        {
            //StartCoroutine(FreezeMovement());
            FreezeMovement();
            Destroy(other.gameObject);
        }
    }

    private void FreezeMovement()
    {
        _isFrozen = true;
        _shakeTextAnimator.enabled = true;
        
        if (_playerMovementScript != null)
        {
            _playerMovementScript.FreezeMovement();
        }

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        GameObject freezeEffect = null;
        if (freezeEffectPrefab != null)
        {
            freezeEffect = Instantiate(freezeEffectPrefab, transform.position, Quaternion.identity);
            freezeEffect.transform.SetParent(transform);
        }
        
        

        SetPlayerColor(Color.cyan);

        /*yield return new WaitForSeconds(freezeDuration);

        SetPlayerColor(originalColor);

        if (freezeEffect != null)
        {
            Destroy(freezeEffect);
        }

        _isFrozen = false;

        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = true;
        }

        if (_rb != null)
        {
            _rb.constraints = _originalConstraints;
        }*/
    }
    
    public void Unfreeze()
    {
        if (_isFrozen)
        {
            _isFrozen = false;
            _shakeTextAnimator.SetTrigger("disableShake");
            _shakeText.SetActive(false);
            _shakeTextAnimator.enabled = false;
            SetPlayerColor(originalColor);
            

            if (_playerMovementScript != null)
                _playerMovementScript.UnfreezeMovement();

            if (_rb != null)
                _rb.constraints = _originalConstraints;

            // Destroy freeze effect if exists
            foreach (Transform child in transform)
            {
                if (child.gameObject.CompareTag("FreezeEffect"))
                    Destroy(child.gameObject);
            }
        }
    }

    private void SetPlayerColor(Color color)
    {
        foreach (Renderer renderer in _playerRenderers)
        {
            if (renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = color;
            }
        }
    }
}
