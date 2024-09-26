using UnityEngine;
using System.Collections;

public class ShootingSystem : MonoBehaviour
{
    public float bulletSpeed = 20.0f;
    public GameObject volumetricLine;

    private InputManager _inputManager;
    [SerializeField] private AudioClip[] shootClips;
    //public GameObject spaceship;
    
    /*public void Initialize(InputManager inputManager)
    {
        Debug.Log("running");
        _inputManager = inputManager;
    }*/

    public void Initialize(InputManager inputManager)
    {
        _inputManager = inputManager;

        _inputManager.OnShoot += Shoot;
    }
    void OnDestroy()
    {
        if (_inputManager != null)
        {
            _inputManager.OnShoot -= Shoot;
        }
    }

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
        GameObject bullet = Instantiate(volumetricLine, this.transform.position, this.transform.rotation);

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
        rb.velocity = this.transform.forward * bulletSpeed;

        Destroy(bullet, 5.0f);

        Debug.Log("ShootingSystem: Shoot() called with Volumetric Line");
    }
}
