using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;
using System;



public class ShootingSystem : MonoBehaviour
{
    public float bulletSpeed = 35.0f;
    public GameObject volumetricLine;

    private InputManager _inputManager;
    [SerializeField] private AudioClip[] shootClips;

    private GameObject _lineRendererObject;
    private LineRendererScript _lineRenderer;
    //public GameObject spaceship;
    
    /*public void Initialize(InputManager inputManager)
    {
        Debug.Log("running");
        _inputManager = inputManager;
    }*/

    private bool _aimFocusOn;
    private GameObject _aimObject;
    
    private List<GameObject>_targetObjects;

    public Vector3 aimAssistSensitivity = new Vector3(2.0f, 2.0f, 35.0f);

    private GameObject _obstacleManagerObject;
    private ObstacleManager _obstacleManager;

    public Action<GameObject> onAimObjectDestroyed;

    public void Initialize(InputManager inputManager)
    {
        _aimFocusOn = false;
        
        _obstacleManagerObject = GameObject.Find("SpawnPlane");
        _obstacleManager = _obstacleManagerObject.GetComponent<ObstacleManager>();
        
        
        _inputManager = inputManager;

        _inputManager.OnShoot += Shoot;

        _lineRendererObject = GameObject.Find("LineRenderer");
        _lineRenderer = _lineRendererObject.GetComponent<LineRendererScript>();

        _obstacleManager.onAsteroidsChange += obstacleChange;
        _aimObject = null;
        
        StartCoroutine(aimAssistCheck());

       
    }

    private void Awake()
    {
        onAimObjectDestroyed += AimObjectDestroyed;
    }
    
    

    void OnDestroy()
    {
        onAimObjectDestroyed -= AimObjectDestroyed;
        if (_inputManager != null) _inputManager.OnShoot -= Shoot;
        if(_obstacleManager != null) _obstacleManager.onAsteroidsChange -= obstacleChange;
    }
    
    
    void obstacleChange()
    {
        _targetObjects = _obstacleManager.asteroids;
    }

    IEnumerator aimAssistCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);

            float bestValue = 10000;
            GameObject bestTarget = null;

            if (_targetObjects == null) continue;
            foreach (var target in _targetObjects)
            {
                Vector3 diffXYZ = target.transform.position - transform.position;
                float distanceXY = new Vector2(diffXYZ.x, diffXYZ.y).magnitude;

                if (distanceXY < aimAssistSensitivity.x && diffXYZ.z < aimAssistSensitivity.z && diffXYZ.z > 1.0f)
                {
                    if (bestValue > diffXYZ.z)
                    {
                        bestValue = diffXYZ.z;
                        bestTarget = target;
                    }
                }
            }

            if(bestTarget == null){ // nothing found
                if(_aimObject != null) {
                    _lineRenderer.removeWireCube(_aimObject);
                    _aimFocusOn = false;
                }
            }
            else if (!_aimFocusOn)
            {
                _aimObject = bestTarget;
                onNewAimObject();
                _aimFocusOn = true;
                _lineRenderer.addWireCube(_aimObject, gameObject);
            }
            else if (_aimObject != bestTarget)
            {
                _lineRenderer.removeWireCube(_aimObject);
                _aimObject = bestTarget;
                onNewAimObject();
                _lineRenderer.addWireCube(_aimObject, gameObject);
            }
        }
    }

    private void onNewAimObject()
    {
        _aimObject.GetComponent<Obstacle>().addOnDestroyed(onAimObjectDestroyed);
    }
    private void AimObjectDestroyed(GameObject astr)
    {
        _aimFocusOn = false;
        _lineRenderer.removeWireCube(astr);
    }

    public void Shoot()
    {
        Debug.Log("Shootin");
        Vector3 shootPosition  = this.transform.position + new Vector3(-2.0f, 0.5f, 0.0f);

        Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);

        if (_aimFocusOn)
        {
            Vector3 diffVector  = _aimObject.transform.position - shootPosition;
            velocity = bulletSpeed * Vector3.Normalize(diffVector);
        }
        else
        {
            velocity = this.transform.forward * bulletSpeed;
        }
        
        Quaternion bulletRotation = Quaternion.LookRotation(velocity);
        GameObject bullet = Instantiate(volumetricLine, shootPosition, bulletRotation);
        bullet.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        SoundFXManager.instance.PlayRandomSoundFXClip(shootClips, transform, 0.5f);


        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Collider collider = bullet.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        bullet.AddComponent<Bullet>();
        
        rb.velocity = velocity;

        Destroy(bullet, 5.0f);

    }
}
