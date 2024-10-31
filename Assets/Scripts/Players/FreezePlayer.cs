using System.Collections;
using UnityEngine;

public class FreezePlayer : MonoBehaviour
{
    public float freezeDuration = 3f;
    public Shader freezeShader;
    public Renderer[] additionalRenderers;
    private bool _isFrozen;
    private Renderer[] _playerRenderers;
    private PlayerMovementNEW _playerMovementScript;
    private Rigidbody _rb;
    private RigidbodyConstraints _originalConstraints;

    private Shader[] originalShaders;

    void Start()
    {
        _isFrozen = false;

        var childRenderers = GetComponentsInChildren<Renderer>(true);
        int totalRenderers = childRenderers.Length + (additionalRenderers != null ? additionalRenderers.Length : 0);

        _playerRenderers = new Renderer[totalRenderers];
        childRenderers.CopyTo(_playerRenderers, 0);

        if (additionalRenderers != null && additionalRenderers.Length > 0)
        {
            additionalRenderers.CopyTo(_playerRenderers, childRenderers.Length);
        }

        _playerMovementScript = GetComponent<PlayerMovementNEW>();
        _rb = GetComponent<Rigidbody>();
        _originalConstraints = _rb.constraints;

        originalShaders = new Shader[_playerRenderers.Length];
        for (int i = 0; i < _playerRenderers.Length; i++)
        {
            if (_playerRenderers[i].material != null)
            {
                originalShaders[i] = _playerRenderers[i].material.shader;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("frozen") && !_isFrozen)
        {
            StartCoroutine(FreezeMovement());
            Destroy(other.gameObject);
        }
    }

    private IEnumerator FreezeMovement()
    {
        _isFrozen = true;

        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = false;
        }

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        SetPlayerShader(freezeShader);

        yield return new WaitForSeconds(freezeDuration);

        RestoreOriginalShaders();

        _isFrozen = false;

        if (_playerMovementScript != null)
        {
            _playerMovementScript.enabled = true;
        }

        if (_rb != null)
        {
            _rb.constraints = _originalConstraints;
        }
    }

    private void SetPlayerShader(Shader shader)
    {
        foreach (Renderer renderer in _playerRenderers)
        {
            if (renderer.material != null)
            {
                renderer.material.shader = shader;
            }
        }
    }

    private void RestoreOriginalShaders()
    {
        for (int i = 0; i < _playerRenderers.Length; i++)
        {
            if (_playerRenderers[i].material != null)
            {
                _playerRenderers[i].material.shader = originalShaders[i];
            }
        }
    }
}
