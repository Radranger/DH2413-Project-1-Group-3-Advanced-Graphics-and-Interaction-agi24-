using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private GameObject _manager;
    private GameManager _gameManager;

    private Vector2 _playerMass;

    private Quaternion _targetRotiation;

    public float rotationAmount = 3.0f;
    public float rotationSpeed = 15.0f;
    // Start is called before the first frame update
    void Start()
    {
        _manager = GameObject.Find("GameManager");
        _gameManager = _manager.GetComponent<GameManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _playerMass = _gameManager.playerMass;
        Debug.Log(_playerMass);

        
        _targetRotiation = Quaternion.Euler(-_playerMass.y*rotationAmount, _playerMass.x*rotationAmount, 0);
        Debug.Log(_targetRotiation);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotiation, rotationSpeed * Time.deltaTime);
    }
}
