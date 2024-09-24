using UnityEngine;
using System.Collections;

public class ShootingSystem : MonoBehaviour
{
    public float bulletSpeed = 20.0f;
    private InputManager _inputManager;
    [SerializeField] private AudioClip[] shootClips;
    //public GameObject spaceship;
    
    /*public void Initialize(InputManager inputManager)
    {
        Debug.Log("running");
        _inputManager = inputManager;
    }*/

    void Start(){
        /*StartCoroutine(Shooting());*/
    }
    

    void Update()
    {
        //_inputManager.GetShooting();
    }

    /*IEnumerator Shooting (){

        while(true){
            Shoot();
            yield return new WaitForSeconds(0.5f);
        }
    }*/
    public void Shoot()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bullet.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        bullet.transform.position = this.transform.position;
        SoundFXManager.instance.PlayRandomSoundFXClip(shootClips, transform, 0.5f);
        //bullet.transform.rotation = spaceship.transform.rotation;

        // Add a rigidbody component so that the bullet is affected by physics
        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        BoxCollider collider = bullet.GetComponent<BoxCollider>();
        collider.isTrigger = true;

        bullet.AddComponent<Bullet>();

        rb.velocity = this.transform.forward * bulletSpeed;
        Destroy(bullet, 5.0f);
        
        Debug.Log("ShootingSystem: Shoot() called");
    }
}
