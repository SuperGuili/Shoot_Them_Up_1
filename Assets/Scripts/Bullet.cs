using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float aliveTime;
    public int damage;
    public float movSpeed;

    private GameObject player;
    private GameObject enemyTriggered;
    private Rigidbody bullet;
    private float playerSpeed;
    private float _aliveTime;

    public GameObject muzzle;

    // Start is called before the first frame update
    private void OnEnable()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player.activeInHierarchy && player != null)
            {
                playerSpeed = player.GetComponent<Player>().speed;
            }
            playerSpeed = player.GetComponent<Player>().speed;

            bullet = GetComponent<Rigidbody>();
            bullet.velocity = transform.up * (movSpeed + playerSpeed);

            /// Muzzle Flash
            muzzle = ObjectPooler.SharedInstance.GetPooledObject("VfxFlash");
            muzzle.transform.SetParent(player.transform);
            muzzle.transform.position = gameObject.transform.position;
            muzzle.SetActive(true);
        }
        catch (System.Exception)
        {
            //throw;
        }
        _aliveTime = 5;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        aliveTime -= 1 * Time.deltaTime;
        if (aliveTime <= 0)
        {
            aliveTime = 5;
            gameObject.SetActive(false);
        }
        if (!player.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {          
            enemyTriggered = other.gameObject;
            Enemy _enemy = enemyTriggered.GetComponent<Enemy>();
            _enemy.RemoveHealth(damage);
            gameObject.SetActive(false);            
        }
    }
}
