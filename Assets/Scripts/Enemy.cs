using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform propeller;
    private GameObject player;
    private Rigidbody rigidbody;

    public float health;

    public GameObject bullet;
    public GameObject SpawnCenterEnemy;

    public float fireRate = 1;
    public int burst = 3;
    private float nextFire = 0;

    private Vector3 dive;
    private bool diving = false;
    private bool corpse = true;
    private GameObject fireFx = null;
    AudioSource audioEngine;
    public AudioClip clipEngine;

    public GameObject hit;

    private void OnEnable()
    {
        propeller = transform.Find("PropellerEnemy");
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = gameObject.GetComponent<Rigidbody>();
        audioEngine = gameObject.GetComponent<AudioSource>();
        audioEngine.clip = clipEngine;
        audioEngine.Play(0);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //////////////////////////////////
        if (!player.activeSelf && transform.position.z < player.transform.position.z + 50)
        {
            Die();
        }

        if (player.transform.position.z - 50 > transform.position.z)
        {
            Die();
        }

        if (health <= 0 && gameObject.activeInHierarchy && corpse)
        {
            propeller.Rotate(0, 0, 1 * 500 * Time.deltaTime);

            if (fireFx != null)
            {
                fireFx.transform.position = transform.position;
            }
            if (!diving)
            {
                player.GetComponent<Player>().score += 10;
                if (player.GetComponent<Player>().gameMode == "hard")
                {
                    player.GetComponent<Player>()._ammoQuantity += 10;
                }

                float x = Random.Range(-20f, 20f);
                dive = new Vector3(x, x, 20);//x - turn right //y - spin clockwise //z - nose down
                diving = true;
            }
            if (transform.position.y > 10)
            {
                // Rotate down the nemy after death             
                Quaternion deltaRotation = Quaternion.Euler(dive * Time.deltaTime);
                rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
                if (dive.x < 0.01f)
                {
                    rigidbody.AddForce(10, 0, 0); // Add force to turn right
                }
                else
                {
                    rigidbody.AddForce(-10, 0, 0); // Add force to turn left
                }
                rigidbody.useGravity = true; // Gravity to fall

                // Fire FX
                if (fireFx == null)
                {
                    fireFx = ObjectPooler.SharedInstance.GetPooledObject("VfxFire");
                    fireFx.SetActive(true);
                }
            }
            return;
        }

        if (health >= 1)
        {
            propeller.Rotate(0, 0, 1 * 1500 * Time.deltaTime);
        }

        if (player != null && !diving)
        {
            if (transform.position.x - player.transform.position.x > 0 && transform.position.x
                - player.transform.position.x <= 15 && player.transform.position.z + 20 <= transform.position.z)
            {
                if (Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Invoke("Fire", 0.1f);
                }
            }
            if (transform.position.x - player.transform.position.x < 0 && transform.position.x
                - player.transform.position.x >= -15 && player.transform.position.z + 20 <= transform.position.z)
            {
                if (Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Invoke("Fire", 0.1f);
                }
            }
        }
    }

    public void Die()
    {

        ResetAll();
        gameObject.SetActive(false);
    }

    public void RemoveHealth(int damage)
    {
        health -= damage;
    }

    void Fire()
    {
        bullet = ObjectPooler.SharedInstance.GetPooledObject("BulletEnemy");
        if (bullet != null)
        {
            bullet.transform.position = SpawnCenterEnemy.transform.position;
            bullet.transform.rotation = SpawnCenterEnemy.transform.rotation;
            bullet.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Player>().health -= 2;
            gameObject.SetActive(false);
        }
        if (other.tag == "Terrain")
        {
            corpse = false;
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
            rigidbody.useGravity = false;
            if (transform.position.x > -20) // On the water - Flames off
            {
                Die();
            }
        }
        if (other.tag == "VfxBullet")
        {
            if (player != null)
            {
                HitFx();
            }
        }
    }

    public void ResetAll()
    {
        Vector3 moveVector = new Vector3();
        moveVector.z -= 100;
        health = 4;
        fireRate = 1;
        burst = 3;
        nextFire = 0;
        diving = false;

        rigidbody.useGravity = false;
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.angularVelocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, -90, 10);
        if (fireFx != null)
        {
            fireFx.SetActive(false);
        }
        fireFx = null;
        corpse = true;
        if (hit != null)
        {
            hit.SetActive(false);
        }

        audioEngine.Stop();
    }
    private void HitFx()
    {
        if (gameObject.activeInHierarchy)
        {
            try
            {
                hit = ObjectPooler.SharedInstance.GetPooledObject("VfxImpactEnemy");
                hit.transform.position = transform.position;
                hit.transform.rotation = transform.rotation;
                hit.SetActive(true);
            }
            catch (System.Exception)
            {
                //throw;
            }
        }
    }
}
