using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    private int _level;
    private Rigidbody _rb;
    private ObstacleManager _manager;
    private GameObject[] _astroidObjects;
    private Vector3 _frameVelocity;
    private Vector3 _blastVelocity;
    float CONSTANT_SPEED = -1000.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.AddTorque(new Vector3(0.0f, (float)Random.Range(0.0f, 20.0f), (float)Random.Range(0.0f, 20.0f)));
    }

    public void Initialize(int level, ObstacleManager manager, Vector3 initBlastVelocity = default){
        _level = level;
        _manager = manager;
        _astroidObjects = _manager.astroids;
        if(initBlastVelocity != default) StartCoroutine(BlastMovement(initBlastVelocity));
    }

    IEnumerator BlastMovement(Vector3 initBlastVelocity)
    {

        float decelerationRate = _manager.splitBlastDecelerationRate;
        _blastVelocity = initBlastVelocity;

        while (_blastVelocity.x != 0 || _blastVelocity.y != 0)
        {
            _blastVelocity.x = Mathf.MoveTowards(_blastVelocity.x, 0, decelerationRate * Time.deltaTime);
            _blastVelocity.y = Mathf.MoveTowards(_blastVelocity.y, 0, decelerationRate * Time.deltaTime);

            yield return null;
        }

    }

    public void RegularHit(){
        if(_level != 1) Split();
        Kill();
    }


    void Split(){
        float angleOfSplit = Random.Range(0.0f, 2 * Mathf.PI);
        float xOffset1 = Mathf.Cos(angleOfSplit) * _manager.splitMagnitude;
        float xOffset2 = Mathf.Cos(angleOfSplit + Mathf.PI) * _manager.splitMagnitude;
        float yOffset1 = Mathf.Sin(angleOfSplit) * _manager.splitMagnitude;
        float yOffset2 = Mathf.Sin(angleOfSplit + Mathf.PI) * _manager.splitMagnitude;

        Vector3 spawnPos1 = new Vector3(this.transform.position.x + xOffset1, this.transform.position.y + yOffset1, this.transform.position.z);
        Vector3 spawnPos2 = new Vector3(this.transform.position.x + xOffset2, this.transform.position.y + yOffset2, this.transform.position.z);
        
        Debug.Log(_level-1);
        GameObject astroidInstance1 = Instantiate(_astroidObjects[_level-2], spawnPos1, Quaternion.identity);
        GameObject astroidInstance2 = Instantiate(_astroidObjects[_level-2], spawnPos2, Quaternion.identity);

        Vector3 blastVelocity1 = new Vector3(xOffset1 * _manager.splitBlastMagnitude, yOffset1 * _manager.splitBlastMagnitude, 0.0f);
        Vector3 blastVelocity2 = new Vector3(xOffset2 * _manager.splitBlastMagnitude, yOffset2 * _manager.splitBlastMagnitude, 0.0f);

        astroidInstance1.GetComponent<Obstacle>().Initialize(_level-1, _manager, blastVelocity1);
        astroidInstance2.GetComponent<Obstacle>().Initialize(_level-1, _manager, blastVelocity2);
    }
    void Kill(){
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        FrameReset();
        HandleMovement();
        ApplyMovement();


        if (Input.GetKeyDown(KeyCode.Space))
        {
            RegularHit();
        }
    }
    void FrameReset(){
        _frameVelocity = new Vector3(0.0f,0.0f,0.0f);
    }
    private void HandleMovement()
    {
        _frameVelocity = new Vector3(0.0f, 0.0f, CONSTANT_SPEED * Time.deltaTime);
        _frameVelocity += _blastVelocity;
    }

    private void ApplyMovement() => _rb.velocity = _frameVelocity;
}
