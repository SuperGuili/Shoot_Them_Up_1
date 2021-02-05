using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform propeller;
    private Transform enemies;
    private GameObject playerTransform;
    private Rigidbody rigidbody;

    public float health;

    public GameObject bullet;
    public GameObject SpawnCenterEnemy;
    private GameObject fire;

    public float fireRate = 1;
    public int burst = 3;
    private float nextFire = 0;

    private Vector3 dive;
    private bool diving = true;
    private bool corpse = true;
    private GameObject fireFx;

    private void OnEnable()
    {
        propeller = transform.Find("PropellerEnemy");
        playerTransform = GameObject.FindGameObjectWithTag("Player");
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fireFx = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerTransform.GetComponent<Player>().isAlive)
        {
            Die();
        }

        if (playerTransform.transform.position.z > transform.position.z)
        {
            Die();
        }

        if (health <= 0 && gameObject.activeInHierarchy && corpse)
        {
            if (fireFx != null)
            {
                fireFx.transform.position = transform.position;
            }
            if (transform.position.y > 25)
            {
                // Rotate down the nemy after death
                dive = new Vector3(15, 15, 15);//x - turn right //y - spin clockwise //z - nose down
                Quaternion deltaRotation = Quaternion.Euler(dive * Time.deltaTime);
                rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
                rigidbody.AddForce(-10, 0, 0); // Add force to turn
                rigidbody.useGravity = true; // Gravity to fall

                // Fire FX
                if (fireFx == null)
                {
                    fireFx = ObjectPooler.SharedInstance.GetPooledObject("VfxFire");
                    fireFx.SetActive(true);
                }
                return;
            }

        }
        if (health >= 1)
        {
            propeller.Rotate(0, 0, 1 * 1500 * Time.deltaTime);
        }


        if (playerTransform != null && playerTransform.activeInHierarchy)
        {
            if (transform.position.x - playerTransform.transform.position.x > 0 && transform.position.x - playerTransform.transform.position.x <= 15)
            {
                if (Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    Invoke("Fire", 0.1f);
                }
            }
            if (transform.position.x - playerTransform.transform.position.x < 0 && transform.position.x - playerTransform.transform.position.x >= -15)
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
        if (health <= 0)
        {
            if (playerTransform.GetComponent<Player>().isAlive)
            {
                playerTransform.GetComponent<Player>().score += 10;
            }
        }

        ResetAll();
        gameObject.SetActive(false);
    }

    public void removeHealth(int damage)
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
            playerTransform.GetComponent<Player>().health -= 2;
            gameObject.SetActive(false);
        }
        if (other.tag == "Terrain")
        {
            corpse = false;
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
            rigidbody.useGravity = false;
            if (transform.position.x > -55) // On the water - Flames off
            {
                fireFx.SetActive(false);
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
        diving = true;

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
    }

}
