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
    public float fireRate;
    public int health = 5;
    private int _health;
    private float nextFire = 0;
    public bool isAlive = true;
    public int lives = 3;
    public int score = 0;
    private int scoreUpdater = 0;
    private int level = 0;

    private int limitRight = 250;
    private int limitLeft = -250;

    private GameObject fireFx;
    private bool firefxOn = true;
    private Vector3 moveFireFx;


    // Start is called before the first frame update
    void Start()
    {
        propeller = GameObject.FindGameObjectWithTag("Propeller").transform;
        _rigidbody = GetComponent<Rigidbody>();
        _health = health;
        //fireFx = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (health >= 10)
        {
            propeller.Rotate(0, 0, 1 * 1500 * Time.deltaTime);
        }
        
        if (health <= 9 && firefxOn)
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
            moveFireFx.z += 1.5f;
            fireFx.transform.position = moveFireFx;

        }


        if (health <= 0 && isAlive == true)
        {
            Die();
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
            lives++;
            level++;
            score += 10;
        }

        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }

        if (score != scoreUpdater || score == 0)
        {
            GameManager.Instance._score = score;
            scoreUpdater = score;
        }
        GameManager.Instance._level = level;
        GameManager.Instance._lives = lives;
        GameManager.Instance._health = health;
    }

    private void FixedUpdate()
    {
        float x = 0;
        Vector3 playerPosition = transform.position;
        Vector3 moveVector = new Vector3(x * speed * turnSpeed, 60, speed);

        if (playerPosition.x > -250 && playerPosition.x < 250)
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
    }

    public void removeHealth(int damage)
    {
        health -= damage;
    }

    public void Die()
    {
        isAlive = false;
        lives--;
        if (lives > 0)
        {
            Reset();
        }
    }

    public void Reset()
    {
        health = _health;
        nextFire = 0;
        firefxOn = true;
        fireFx.SetActive(false);
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
    }
}
