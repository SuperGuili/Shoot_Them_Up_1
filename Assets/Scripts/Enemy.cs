using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform propeller;
    private Transform enemies;
    private GameObject playerTransform;

    public float health;

    public GameObject bullet;
    public GameObject SpawnCenterEnemy;

    public float fireRate = 1;
    public int burst = 3;
    private float nextFire = 0;

    private void OnEnable()
    {
        propeller = transform.Find("PropellerEnemy");
        playerTransform = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
        else if (health > 1)
        {
            propeller.Rotate(0, 0, 1 * 1500 * Time.deltaTime);
        }
        if (playerTransform != null)
        {
            if (transform.position.z < playerTransform.transform.position.z && health > 0)
            {
                Die();
            }
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

        if (!playerTransform.GetComponent<Player>().isAlive)
        {
            Die();
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
    }

    public void ResetAll()
    {
        Vector3 moveVector = new Vector3();
        moveVector.z -= 100;
        health = 4;
        fireRate = 1;
        burst = 3;
        nextFire = 0;
    }

}
