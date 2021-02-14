﻿using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform propeller;
    public int speed;
    public int turnSpeed;
    Rigidbody _rigidbody;

    private GameObject bullet;
    public GameObject bulletSpawnLeft;
    public GameObject bulletSpawnRight;
    public float fireRate;
    public int health = 5;
    private int _health;
    private float nextFire = 0;
    public bool isAlive = true;
    public int lives = 3;
    public int score = 0;
    private int scoreUpdater = 0;
    private int level = 1;

    private int limitRight = 260;
    private int limitLeft = -260;

    private GameObject fireFx;
    private bool firefxOn = true;
    private Vector3 moveFireFx;

    private Vector3 dive;
    private bool diving = false;
    private bool corpse = true;

    [SerializeField] public AudioClip engine;
    [SerializeField] public AudioClip gun;
    [SerializeField] public AudioClip impactSound;
    private float engineRPMSound = 1.0f;
    AudioSource soundEngine;
    AudioSource soundGun;
    AudioSource soundImpact;
    GameObject farCloud;

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
    }
    // Update is called once per frame
    void Update()
    {
        if (health >= 3)
        {
            propeller.Rotate(0, 0, 1 * 1500 * Time.deltaTime);
        }

        if (health <= 2)
        {
            propeller.Rotate(0, 0, 1 * 500 * Time.deltaTime);

            if (engineRPMSound >= 0.5f)
            {
                engineRPMSound -= 0.01f;// * Time.deltaTime;
                soundEngine.pitch = engineRPMSound;
            }
        }

        if (health <= 2 && firefxOn)
        {
            fireFx = ObjectPooler.SharedInstance.GetPooledObject("VfxFirePlayer");
            fireFx.SetActive(true);
            firefxOn = false;
        }

        if (!firefxOn)
        {
            // Position of the Fire FX
            moveFireFx = transform.position;
            moveFireFx.y -= 0.5f;
            moveFireFx.z += 2.0f;
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
                float spin = Random.Range(-20f, 20f);
                dive = new Vector3(spin, spin, 20);//x - turn right //y - spin clockwise //z - nose down
                diving = true;
            }
            if (transform.position.y > 10)
            {
                // Rotate down the player after death             
                Quaternion deltaRotation = Quaternion.Euler(dive * Time.deltaTime);
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
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

                if (transform.position.y <= 15) // Under water
                {
                    lives--;
                    GameManager.Instance._lives = lives;
                    Die();
                }

                //Control the cloud attached to the player
                farCloud.transform.rotation = Quaternion.Euler(0, 90.0f, 0.00f);
                farCloud.transform.position = new Vector3(250, 100, playerPosition.z + 1000);
                return;
            }
        }

        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }

        if (playerPosition.x > limitLeft && playerPosition.x < limitRight)
        {
            x = Input.GetAxis("Horizontal"); //Input for X position
        }
        else if (playerPosition.x >= limitRight)
        {
            moveVector.x = playerPosition.x - 5;
            transform.position = new Vector3(moveVector.x, 60, playerPosition.z + 1);
            _rigidbody.AddForce(-1, 0, 0);
            return;
        }
        else if (playerPosition.x <= limitLeft)
        {
            moveVector.x = playerPosition.x + 5;
            transform.position = new Vector3(moveVector.x, 60, playerPosition.z + 1);
            _rigidbody.AddForce(1, 0, 0);
            return;
        }

        moveVector = new Vector3(x * speed * turnSpeed, 60, speed);

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
    }
    public void RemoveHealth(int damage)
    {
        health -= damage;
    }
    public void Die()
    {
        isAlive = false;
        if (lives >= 1)
        {
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
        firefxOn = true;
        fireFx.SetActive(false);

        dive = Vector3.zero;
        diving = false;
        corpse = true;

        engineRPMSound = 1.0f;
        soundEngine.pitch = engineRPMSound;
    }
    public void ResetAll()
    {
        fireFx.SetActive(false);
        firefxOn = true;
        health = _health;
        nextFire = 0;
        fireRate = 0.3f;
        isAlive = true;
        lives = 3;
        score = 0;
        scoreUpdater = 0;
        level = 0;
        transform.position = new Vector3(0, 60, 0);
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 90, 10);
        _rigidbody.useGravity = false;
        dive = Vector3.zero;
        diving = false;
        corpse = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;

        engineRPMSound = 1.0f;
        soundEngine.pitch = engineRPMSound;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            lives--;
            GameManager.Instance._lives = lives;
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
