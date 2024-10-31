using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePlayer : MonoBehaviour
{
    public float freezeDuration = 3f;
    // public GameObject freezeEffectPrefab;
    private bool _isFrozen;
    private Renderer[] _playerRenderers;
    private PlayerMovementNEW _playerMovementScript;
    private Animator _shakeTextAnimator;
    public GameObject _shakeText;
    private Rigidbody _rb;
    private RigidbodyConstraints _originalConstraints;

    // public Color originalColor = Color.white;
    public Shader freezeShader;

    private Dictionary<Renderer, Shader> _originalShaders = new Dictionary<Renderer, Shader>();

    void Start()
    {
        _isFrozen = false;
        _playerRenderers = GetComponentsInChildren<Renderer>();
        _playerMovementScript = GetComponent<PlayerMovementNEW>();
        _shakeTextAnimator = GetComponent<Animator>();
        _shakeTextAnimator.enabled = false;

        _rb = GetComponent<Rigidbody>();
        _originalConstraints = _rb.constraints;

        foreach (Renderer renderer in _playerRenderers)
        {
            if (renderer.material != null)
            {
                _originalShaders[renderer] = renderer.material.shader;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("frozen") && !_isFrozen)
        {
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

        ApplyFreezeShader();
    }

    public void Unfreeze()
    {
        if (_isFrozen)
        {
            _isFrozen = false;
            _shakeTextAnimator.SetTrigger("disableShake");
            _shakeText.SetActive(false);
            _shakeTextAnimator.enabled = false;

            RestoreOriginalShaders();

            if (_playerMovementScript != null)
                _playerMovementScript.UnfreezeMovement();

            if (_rb != null)
                _rb.constraints = _originalConstraints;
        }
    }

    private void ApplyFreezeShader()
    {
        foreach (Renderer renderer in _playerRenderers)
        {
            if (renderer.material != null && freezeShader != null)
            {
                renderer.material.shader = freezeShader;
            }
        }
    }

    private void RestoreOriginalShaders()
    {
        foreach (Renderer renderer in _playerRenderers)
        {
            if (renderer.material != null && _originalShaders.ContainsKey(renderer))
            {
                renderer.material.shader = _originalShaders[renderer];
            }
        }
    }
}
