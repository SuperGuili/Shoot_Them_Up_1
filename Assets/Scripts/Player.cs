using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform propeller;
    public int speed;
    public int turnSpeed;
    Rigidbody _rigidbody;

    private GameObject bullet;
    public GameObject bulletSpawnLeft;
    public GameObject bulletSpawnRight;
    private GameObject vfxSplash;
    private GameObject vfxDustPuff;

    public float fireRate;
    public int health = 10;
    private int _health;
    private float nextFire = 0;
    public bool isAlive = false;
    public int lives = 3;
    public int score = 0;
    private int scoreUpdater = 0;
    private int level = 1;

    private int limitRight = 260;
    private int limitLeft = -260;

    private GameObject fireFx;
    private bool firefxOn = false;
    private Vector3 moveFireFx;

    private Vector3 dive;
    private bool diving = false;

    public AudioClip engine;
    public AudioClip gun;
    public AudioClip impactSound;
    private float engineRPMSound = 1.0f;
    private float prop = 500;

    AudioSource soundEngine;
    AudioSource soundGun;
    AudioSource soundImpact;
    GameObject farCloud;

    public string gameMode;
    public int _ammoQuantity;
    private bool vfxSplashOn = false;
    private bool vfxDustPuffOn = false;
    

    // Start is called before the first frame update
    void Start()
    {
        propeller = GameObject.FindGameObjectWithTag("Propeller").transform;
        _rigidbody = GetComponent<Rigidbody>();
        _health = health;

        //Sound Fxs
        soundEngine = gameObject.AddComponent<AudioSource>();
        soundEngine.clip = engine;
        soundEngine.volume = 0.3f;
        soundEngine.loop = true;
        soundEngine.Play();

        soundGun = gameObject.AddComponent<AudioSource>();
        soundGun.clip = gun;
        soundGun.volume = 0.4f;
        soundGun.pitch = 1;
        soundGun.playOnAwake = false;

        soundImpact = gameObject.AddComponent<AudioSource>();
        soundImpact.clip = impactSound;
        soundImpact.pitch = 2;
        soundImpact.playOnAwake = false;

        farCloud = GameObject.FindGameObjectWithTag("Steam");

        gameMode = GameManager.Instance.gameMode;
        _ammoQuantity = 1000;
    }
    // Update is called once per frame
    void Update()
    {
        if (health >= 3)
        {
            propeller.Rotate(0, 0, 1 * 1000 * Time.deltaTime);
        }

        if (health <= 2 && health >= 1)
        {
            propeller.Rotate(0, 0, 1 * 500 * Time.deltaTime);

            if (engineRPMSound >= 0.5f)
            {
                engineRPMSound -= 0.01f;
                soundEngine.pitch = engineRPMSound;
            }
        }

        if (health <= 2 && !firefxOn)
        {
            fireFx = ObjectPooler.SharedInstance.GetPooledObject("VfxFirePlayer");
            fireFx.SetActive(true);
            firefxOn = true;
        }

        if (firefxOn)
        {
            // Position of the Fire FX
            moveFireFx = transform.position;
            moveFireFx.y -= 0.5f;
            moveFireFx.z += 3.0f;
            fireFx.transform.position = moveFireFx;
        }

        if (score == 50)
        {
            fireRate -= 0.1f;
            speed += 10;
            turnSpeed += 1;
            score += 10;
        }
        if (score == 100)
        {
            fireRate -= 0.1f;
            speed += 10;
            turnSpeed += 1;
            score += 10;
        }
        if (score == 1000)
        {
            fireRate -= 0.05f;
            lives++;
            level++;
            score += 10;
        }
    }
    private void FixedUpdate()
    {
        if (score != scoreUpdater || score == 0)
        {
            GameManager.Instance._score = score;
            scoreUpdater = score;
        }
        GameManager.Instance._level = level;
        GameManager.Instance._lives = lives;
        GameManager.Instance._health = health;
        GameManager.Instance._ammoQuantity = _ammoQuantity;

        float x = 0;
        Vector3 playerPosition = transform.position;
        Vector3 moveVector = new Vector3(x * speed * turnSpeed, 60, speed);

        if (health <= 0 && isAlive)
        {
            if (engineRPMSound >= 0.0f)
            {
                engineRPMSound -= 0.1f * Time.deltaTime;
                soundEngine.pitch = engineRPMSound;
            }
            if (!diving)
            {
                ///camera change
                GameManager.Instance.CameraTop();

                float spin = Random.Range(-20f, 20f);
                dive = new Vector3(spin, spin, 20);//x - turn right //y - spin clockwise //z - nose down
                diving = true;
            }
            if (transform.position.y > 20)
            {
                // Rotate the player down after death                
                _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(dive * Time.deltaTime));
                if (dive.x < 0.01f)
                {
                    _rigidbody.AddForce(-10, 0, 0); // Add force to turn left
                }
                else
                {
                    _rigidbody.AddForce(10, 0, 0); // Add force to turn right
                }
                _rigidbody.constraints = RigidbodyConstraints.None;
                _rigidbody.useGravity = true; // Gravity to fall

                //slow the proppeler                
                propeller.Rotate(0, 0, 1 * prop * Time.deltaTime);
                prop -= 2 * Time.deltaTime;

            }            

            //Splash FX
            if (transform.position.x > -20 && transform.position.y <= 20 && !vfxSplashOn)
            {
                vfxSplash = ObjectPooler.SharedInstance.GetPooledObject("VfxSplash");
                vfxSplash.transform.position = transform.position;
                fireFx.SetActive(false);
                vfxSplash.SetActive(true);
                vfxSplashOn = true;
            }
            // Smoke FX
            if (transform.position.y <= 40 && !vfxDustPuffOn) 
            {    
                vfxDustPuff = ObjectPooler.SharedInstance.GetPooledObject("VfxDust");
                vfxDustPuff.transform.position = transform.position;
                vfxDustPuff.transform.SetParent(transform);
                vfxDustPuff.SetActive(true);
                vfxDustPuffOn = true;
            }

            //Control the cloud attached to the player
            farCloud.transform.rotation = Quaternion.Euler(0, 90.0f, 0.00f);
            farCloud.transform.position = new Vector3(250, 100, playerPosition.z + 1000);

            if (transform.position.y <= 12) // Under water
            {
                Die();
            }

            return;
        }

        if (playerPosition.x > limitLeft && playerPosition.x < limitRight)
        {
            x = Input.GetAxis("Horizontal"); //Input for X position
        }
        else if (playerPosition.x >= limitRight)
        {
            moveVector.x = playerPosition.x - 5;
            transform.position = new Vector3(moveVector.x, 0, playerPosition.z + 1);
            _rigidbody.AddForce(-1, 0, 0);
            return;
        }
        else if (playerPosition.x <= limitLeft)
        {
            moveVector.x = playerPosition.x + 5;
            transform.position = new Vector3(moveVector.x, 0, playerPosition.z + 1);
            _rigidbody.AddForce(1, 0, 0);
            return;
        }

        moveVector = new Vector3(x * speed * turnSpeed, 0, speed);

        _rigidbody.AddForce(moveVector);

        //////// Turn rotation
        float temp = transform.rotation.x;
        temp = 100 * temp;

        if (x > 0 && temp < 20)
        {
            _rigidbody.transform.Rotate(0.5f, 0, 0);
        }
        if (x < 0 && temp > -8)
        {
            _rigidbody.transform.Rotate(-0.5f, 0, 0);
        }
        if (x == 0 && temp <= 21 && temp > 6.1)
        {
            _rigidbody.transform.Rotate(-0.5f, 0, 0);
        }
        if (x == 0 && temp >= -21 && temp < 5.9)
        {
            _rigidbody.transform.Rotate(0.5f, 0, 0);
        }

        //Control the cloud attached to the player
        farCloud.transform.rotation = Quaternion.Euler(0, 90.0f, 0.00f);
        farCloud.transform.position = new Vector3(250, 100, playerPosition.z + 1000);

        //Waiting for input to fire the gun
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (gameMode != "hard")
            {
                Fire();
            }
            if (gameMode == "hard" && _ammoQuantity > 0)
            {
                Fire();
                GameManager.Instance._ammoQuantity = _ammoQuantity;
            }
        }
    }
    void Fire()
    {
        bullet = ObjectPooler.SharedInstance.GetPooledObject("VfxBullet");
        if (bullet != null)
        {
            bullet.transform.position = bulletSpawnLeft.transform.position;
            bullet.transform.rotation = bulletSpawnLeft.transform.rotation;
            bullet.SetActive(true);
        }
        bullet = ObjectPooler.SharedInstance.GetPooledObject("VfxBullet");
        if (bullet != null)
        {
            bullet.transform.position = bulletSpawnRight.transform.position;
            bullet.transform.rotation = bulletSpawnRight.transform.rotation;
            bullet.SetActive(true);
        }
        soundGun.PlayOneShot(gun, 0.3f);

        if (gameMode == "hard")
        {
            _ammoQuantity -= 2;
        }
    }
    public void RemoveHealth(int damage)
    {
        health -= damage;
    }
    public void Die()
    {
        isAlive = false;
        GameManager.Instance._isAlive = isAlive;
        if (lives >= 1)
        {
            lives--;
            GameManager.Instance._lives = lives;
            Reset();
        }

    }
    public void Reset()
    {
        _rigidbody.useGravity = false;
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 90, 10);
        transform.position = new Vector3(0, 60, transform.position.z);
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;

        health = _health;

        nextFire = 0;
        firefxOn = false;
        fireFx.SetActive(false);

        dive = Vector3.zero;
        diving = false;

        engineRPMSound = 1.0f;
        soundEngine.pitch = engineRPMSound;
        prop = 500;

        vfxSplashOn = false;
        vfxDustPuffOn = false;
        if (vfxDustPuff != null)
        {
            Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
            vfxDustPuff.transform.SetParent(VFX);
            vfxDustPuff.SetActive(false);
        }
    }
    public void ResetAll()
    {
        fireFx.SetActive(false);
        firefxOn = false;
        health = _health;
        nextFire = 0;
        fireRate = 0.3f;
        lives = 3;
        score = 0;
        scoreUpdater = 0;
        level = 1;
        transform.position = new Vector3(0, 60, 0);
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 90, 10);
        _rigidbody.useGravity = false;
        dive = Vector3.zero;
        diving = false;

        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;

        engineRPMSound = 1.0f;
        soundEngine.pitch = engineRPMSound;
        prop = 500;

        gameMode = GameManager.Instance.gameMode;
        _ammoQuantity = 1000;

        vfxSplashOn = false;
        vfxDustPuffOn = false;

        if (vfxDustPuff != null)
        {
            Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
            vfxDustPuff.transform.SetParent(VFX);
            vfxDustPuff.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {            
            Die();
        }

        if (other.tag == "BulletEnemy")
        {
            soundImpact.PlayOneShot(impactSound, 0.5f);
        }
    }
    private void OnDisable()
    {
        soundEngine.Stop();        
    }
}
