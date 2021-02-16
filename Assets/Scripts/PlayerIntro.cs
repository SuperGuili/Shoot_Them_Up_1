using UnityEngine;

public class PlayerIntro : MonoBehaviour
{
    private Transform propeller;
    public int speed;
    public int turnSpeed;
    Rigidbody _rigidbody;

    public GameObject bullet;
    public GameObject bulletSpawnLeft;
    public GameObject bulletSpawnRight;
    public float fireRate;
    private float nextFire = 0.5f;
    public bool isAlive = true;

    private int limitRight = 260;
    private int limitLeft = -260;

    [SerializeField] public AudioClip engine;
    [SerializeField] public AudioClip gun;
    AudioSource soundEngine;
    AudioSource soundGun;

    // Start is called before the first frame update
    void Start()
    {
        propeller = GameObject.FindGameObjectWithTag("Propeller").transform;
        _rigidbody = GetComponent<Rigidbody>();

        //Sound Fxs
        soundEngine = gameObject.AddComponent<AudioSource>();
        soundEngine.clip = engine;
        soundEngine.volume = 0.5f;
        soundEngine.loop = true;
        soundEngine.spatialBlend = 0.8f;
        soundEngine.Play();

        soundGun = gameObject.AddComponent<AudioSource>();
        soundGun.clip = gun;
        soundGun.volume = 0.5f;
        soundGun.pitch = 1;
        soundGun.playOnAwake = false;
        soundGun.loop = false;
        soundGun.spatialBlend = 0.8f;

    }
    // Update is called once per frame
    void Update()
    {
        propeller.Rotate(0, 0, 1 * 1500 * Time.deltaTime);

    }
    private void FixedUpdate()
    {

        float x = 0;
        Vector3 playerPosition = transform.position;
        Vector3 moveVector = new Vector3(0, 0, 50);

        _rigidbody.AddForce(moveVector);

        // Rotate down the player after death             
        Quaternion deltaRotation = Quaternion.Euler(8 * Time.deltaTime, 5 * Time.deltaTime, 0);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);

        _rigidbody.AddForce(10, 0, 0); // Add force to turn right

        //_rigidbody.constraints = RigidbodyConstraints.None;



        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }

    }
    void Fire()
    {
        bullet = Instantiate(bullet, bulletSpawnLeft.transform.position, bulletSpawnLeft.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 150);

        bullet = Instantiate(bullet, bulletSpawnRight.transform.position, bulletSpawnRight.transform.rotation);
        rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 150);

        soundGun.PlayOneShot(gun, 0.3f);
    }

    private void OnDisable()
    {
        soundEngine.Stop();
    }
}
