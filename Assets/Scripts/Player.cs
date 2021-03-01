using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform propeller; // transform to animate the propeller
    public int speed = 20; // player speed
    public int turnSpeed = 10; // player turn speed
    Rigidbody _rigidbody; // player rigid body
    private GameObject enemies; // used to change quantity of enemies in wave

    private GameObject bullet; // Object to load the bullet object to fire the gun
    public GameObject bulletSpawnLeft; // Object to spawn the bullets
    public GameObject bulletSpawnRight; // Object to spawn the bullets
    private GameObject vfxSplash; // Object to load the water splash fx
    private GameObject vfxDustPuff; // Object to load the smoke fx

    public float fireRate = 0.5f; // Shots per second
    private float nextFire = 0; // Used to count time for firerate
    public int _health = 10; // Player health    
    public bool isAlive = false; // If the player is alive
    public int lives = 3; // Number of lives
    public int score = 0; // Score
    private int scoreUpdater = 0; //Used to update the panel play on gamemanager script
    private int level = 1; //Player level

    private int limitRight = 270; // Limit of the map
    private int limitLeft = -270; // Limit of the map

    private GameObject fireFx; // Object to load the fire FX
    private bool firefxOn = false; // to check firefx
    private Vector3 moveFireFx; // used to update firefx position

    private Vector3 dive; // used to rotate player after death
    private bool diving = false; // used to run the animation when dead 

    public AudioClip engine; // used to load the sound fx
    public AudioClip gun; // used to load the sound fx
    public AudioClip impactSound; // used to load the sound fx
    private float engineRPMSound = 1.0f; // used to control the sound pitch of engine
    private float prop = 1000; // used to control the propeller rotation speed

    AudioSource soundEngine; // used to add audio source 
    AudioSource soundGun; // used to add audio source 
    AudioSource soundImpact; // used to add audio source 
    GameObject farCloud; // used to load the Object farCloud, the cloud attached to player

    public string gameMode; // used to store the game mode, "normal" or "hard"
    public int _ammoQuantity; // Ammunition quantity for game mode "hard"
    private bool vfxSplashOn = false; // used to control the splash fx
    private bool vfxDustPuffOn = false; // used to control the smoke fx

    private float x = 0; // used to ajust player position
    private Vector3 moveVector; // used to change player position
    float rotation; // used to correct player rotation

    // Start is called before the first frame update
    void Start()
    {
        propeller = GameObject.FindGameObjectWithTag("Propeller").transform;
        _rigidbody = GetComponent<Rigidbody>();

        //Sound Fxs
        soundEngine = gameObject.AddComponent<AudioSource>();
        soundEngine.clip = engine;
        soundEngine.volume = 0.3f;
        soundEngine.loop = true;
        soundEngine.Play();
        //Sound Fxs
        soundGun = gameObject.AddComponent<AudioSource>();
        soundGun.clip = gun;
        soundGun.volume = 0.4f;
        soundGun.pitch = 1;
        soundGun.playOnAwake = false;
        //Sound Fxs
        soundImpact = gameObject.AddComponent<AudioSource>();
        soundImpact.clip = impactSound;
        soundImpact.pitch = 2;
        soundImpact.playOnAwake = false;

        //Cloud
        farCloud = GameObject.FindGameObjectWithTag("Steam");

        // Update Game Manager
        _ammoQuantity = 1000;
        gameMode = GameManager.Instance.gameMode;
        GameManager.Instance._lives = lives;
        GameManager.Instance._level = level;
        GameManager.Instance._health = _health;
        GameManager.Instance._ammoQuantity = _ammoQuantity;

        enemies = GameObject.FindGameObjectWithTag("Enemies");
    }
    // Update is called once per frame
    void Update()
    {
        if (score != scoreUpdater) // update the score on game manager
        {
            GameManager.Instance._score = score;
            scoreUpdater = score;
        }

        if (_health >= 3)
        {
            propeller.Rotate(0, 0, 2 * prop * Time.deltaTime); //Propeller animation 1000RPM
        }

        if (_health <= 2 && _health >= 1)
        {
            propeller.Rotate(0, 0, 1 * prop * Time.deltaTime); //Propeller animation 500 RPM

            if (engineRPMSound >= 0.5f) // Slow decrease of rpm sound
            {
                engineRPMSound -= 0.01f;
                soundEngine.pitch = engineRPMSound;
            }
        }

        if (_health <= 2 && !firefxOn) // turn on fire fx
        {
            fireFx = ObjectPooler.SharedInstance.GetPooledObject("VfxFirePlayer");
            fireFx.SetActive(true);
            firefxOn = true;
        }

        if (firefxOn) // update fire fx position
        {
            // Position of the Fire FX
            moveFireFx = transform.position;
            moveFireFx.y -= 0.5f;
            moveFireFx.z += 3.0f;
            fireFx.transform.position = moveFireFx;
        }

        // Logic for difficulty
        if (score == level * 500)
        {
            fireRate /= 2;
            speed += 5;
            score += 10;
            enemies.GetComponent<Enemies>().wave += 2;
        }
        if (score == level * 1000)
        {
            fireRate /= 1.5f;
            speed += 5;
            score += 10;
            enemies.GetComponent<Enemies>().wave += 2;
        }
        if (score == level * 2000)
        {
            fireRate /= 1.5f;
            speed += 5;
            score += 10;
            enemies.GetComponent<Enemies>().wave += 2;
            enemies.GetComponent<Enemies>().NextWave(-1);
        }
        if (score == level * 5000)
        {            
            if (fireRate > 0.04f)
            {
                fireRate /= 1.5f;
                speed += 5;
            }
            
            lives++;
            GameManager.Instance._lives = lives;
            level++;
            GameManager.Instance._level = level;
            score += 10;
            enemies.GetComponent<Enemies>().wave += 2;
            enemies.GetComponent<Enemies>().NextWave(-1);
        }
        // Logic for difficulty
    }
    private void FixedUpdate()
    {
        rotation = transform.rotation.x * 100; // Player rotation

        if (_health <= 0 && isAlive)
        {
            if (engineRPMSound >= 0.0f) // slow the pitch for engine sound
            {
                engineRPMSound -= 0.1f * Time.deltaTime;
                soundEngine.pitch = engineRPMSound;
            }
            if (!diving) // get random angles to rotate to death
            {
                ///camera change
                GameManager.Instance.CameraTop();

                float spin = Random.Range(-20f, 20f);
                dive = new Vector3(spin, spin, 15);//x+ - turn right //y+ - spin clockwise //z+ - nose down
                diving = true;
            }
            if (transform.position.y > 20)
            {
                // Rotate the player down after death                
                _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(dive * Time.deltaTime));
                if (dive.x < 0.01f)
                {
                    _rigidbody.AddForce(-10, -11, speed / 2); // Add force to turn left and down
                }
                else
                {
                    _rigidbody.AddForce(10, -11, speed / 2); // Add force to turn right and down
                }
                _rigidbody.constraints = RigidbodyConstraints.None; // remove constrains

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
            farCloud.transform.position = new Vector3(250, 80, transform.position.z + 1000);

            if (transform.position.y <= 12) // Under water
            {
                Die();
            }

            return;
        }

        if (transform.position.x > limitLeft && transform.position.x < limitRight) // check for limits
        {
            x = Input.GetAxis("Horizontal"); //Input for X position inside limits
            moveVector = new Vector3(x * speed * turnSpeed, 60, speed);

            _rigidbody.AddForce(moveVector);

            //////// Turn rotation and correction
            if (x > 0 && rotation < 20)
            {
                _rigidbody.transform.Rotate(0.5f, 0, 0);
            }
            if (x < 0 && rotation > -8)
            {
                _rigidbody.transform.Rotate(-0.5f, 0, 0);
            }
            if (x == 0 && rotation <= 21 && rotation > 6.1)
            {
                _rigidbody.transform.Rotate(-0.5f, 0, 0);
            }
            if (x == 0 && rotation >= -21 && rotation < 5.9)
            {
                _rigidbody.transform.Rotate(0.5f, 0, 0);
            }

        }
        else if (transform.position.x >= limitRight)
        {
            moveVector.x = transform.position.x - 1;
            transform.position = new Vector3(moveVector.x, 60, transform.position.z);
            _rigidbody.AddForce(-100, 0, speed);
            if (rotation > -8)
            {
                _rigidbody.transform.Rotate(-0.5f, 0, 0);
            }
        }
        else if (transform.position.x <= limitLeft)
        {
            moveVector.x = transform.position.x + 1;
            transform.position = new Vector3(moveVector.x, 60, transform.position.z);
            _rigidbody.AddForce(100, 0, speed);
            if (rotation < 20)
            {
                _rigidbody.transform.Rotate(0.5f, 0, 0);
            }
        }

        //Control the cloud attached to the player
        farCloud.transform.rotation = Quaternion.Euler(0, 90.0f, 0.00f);
        farCloud.transform.position = new Vector3(250, 80, transform.position.z + 1000);

        //Waiting for input to shoot
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (gameMode != "hard")
            {
                Fire();
            }
            if (gameMode == "hard" && _ammoQuantity >= 1)
            {
                Fire();
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
            GameManager.Instance._ammoQuantity = _ammoQuantity;
        }
    }
    public void RemoveHealth(int damage)
    {
        if (_health >= 1)
        {
            _health -= damage;
            GameManager.Instance._health = _health;
        }
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

        _health = 10;
        GameManager.Instance._health = _health;
        nextFire = 0;
        firefxOn = false;
        fireFx.SetActive(false);

        dive = Vector3.zero;
        diving = false;

        engineRPMSound = 1.0f;
        soundEngine.pitch = engineRPMSound;
        prop = 1000;

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
        speed = 20;
        turnSpeed = 10;
        fireFx.SetActive(false);
        firefxOn = false;
        _health = 10;
        GameManager.Instance._health = _health;
        nextFire = 0;
        fireRate = 0.5f;
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
        prop = 1000;

        gameMode = GameManager.Instance.gameMode;
        _ammoQuantity = 1000;
        GameManager.Instance._ammoQuantity = _ammoQuantity;
        GameManager.Instance._lives = lives;
        GameManager.Instance._level = level;
        GameManager.Instance._health = _health;
        GameManager.Instance._score = score;

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
