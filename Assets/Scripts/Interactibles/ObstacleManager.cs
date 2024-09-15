using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    
    // Start is called before the first frame update
    void Start()
    {
        _spawnDim = _spawnPlane.GetComponent<Renderer>().bounds.size;
        _spawnPos = _spawnPlane.transform.position;

        SpawnInstance();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn(){

        while (true){
            yield return new WaitForSeconds(_spawnInterval);
            //int amount = Mathf.Max(0, (int)Random.Range(Mathf.Floor(_spawnRate/60.0f)-2, Mathf.Floor(_spawnRate/60.0f)+2));

            for(int i = 0; i < 1; i++){
                //SpawnInstance();
            }

        }
    }
    void SpawnInstance(){
        Vector3 spawnPos = new Vector3(_spawnPos.x + Random.Range(-_spawnDim.x / 2, _spawnDim.x / 2), _spawnPos.y + Random.Range(-_spawnDim.y / 2, _spawnDim.y / 2), _spawnPos.z);
        int spawnLevel = Random.Range(1, astroids.Length) + 1;
        GameObject astroidInstance = Instantiate(astroids[spawnLevel - 1], spawnPos, Quaternion.identity);
        astroidInstance.GetComponent<Obstacle>().Initialize(spawnLevel, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnInstance();
        }
        
    }
}
