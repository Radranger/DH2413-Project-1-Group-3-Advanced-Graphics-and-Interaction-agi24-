using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private float _spawnRate = 60.0f;
    [SerializeField] private int _spawnInterval = 1;
    [SerializeField] private GameObject _spawnPlane;
    private Vector3 _spawnDim;
    private Vector3 _spawnPos; 
    public GameObject[] astroids;
    public float splitMagnitude = 10.0f;
    public float splitBlastMagnitude = 5.0f;
    public float splitBlastDecelerationRate = 10.0f;

    private GameObject _lineRenderer; 
    private LineRendererScript _lineRendererScript; 
    
    public List<GameObject> asteroids = new List<GameObject>();
    public Action onAsteroidsChange;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GameObject.Find("LineRenderer");
        _lineRendererScript = _lineRenderer.GetComponent<LineRendererScript>();

        _spawnDim = _spawnPlane.GetComponent<Renderer>().bounds.size;
        _spawnPos = _spawnPlane.transform.position;
    }

    public void startSpawning()
    {
        SpawnInstance();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn(){

        while (true){
            yield return new WaitForSeconds(_spawnInterval);
            //int amount = Mathf.Max(0, (int)Random.Range(Mathf.Floor(_spawnRate/60.0f)-2, Mathf.Floor(_spawnRate/60.0f)+2));

            for(int i = 0; i < 1; i++){
                SpawnInstance();
            }

        }
    }
    void SpawnInstance(){
        Vector3 spawnPos = new Vector3(_spawnPos.x + UnityEngine.Random.Range(-_spawnDim.x / 2, _spawnDim.x / 2), _spawnPos.y + UnityEngine.Random.Range(-_spawnDim.y / 2, _spawnDim.y / 2), _spawnPos.z);
        int spawnLevel = UnityEngine.Random.Range(1, astroids.Length) + 1;
        GameObject asteroidInstance = Instantiate(astroids[spawnLevel - 1], spawnPos, Quaternion.identity);
        
        // Action<GameObject> onAsteroidDestroyed = (destroyedAsteroid) =>
        // {
        //     Debug.Log("Destroyd! ");
        //     _lineRendererScript.removeWireCube(destroyedAsteroid);
        // };
        asteroidInstance.GetComponent<Obstacle>().Initialize(spawnLevel, this, Vector3.zero);
        //_lineRendererScript.addWireCube(asteroidInstance);
        
        asteroids.Add(asteroidInstance);
        
        onAsteroidsChange?.Invoke();
    }

    

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     SpawnInstance();
        // }
        
    }
}
