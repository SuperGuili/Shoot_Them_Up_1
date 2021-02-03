using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float aliveTime;
    public int damage;
    public float movSpeed;

    private GameObject enemyTriggered;
    private Rigidbody bullet;
    private float playerSpeed;
    private float _aliveTime = 20;

    public GameObject muzzle;
    public GameObject hit;

    // Start is called before the first frame update
    private void OnEnable()
    {
        try
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player.activeInHierarchy && player != null)
            {
                playerSpeed = player.GetComponent<Player>().speed;
            }
            playerSpeed = player.GetComponent<Player>().speed;

            bullet = GetComponent<Rigidbody>();
            bullet.velocity = transform.up * (movSpeed + playerSpeed);
            /// Muzzle Flash
            muzzle = ObjectPooler.SharedInstance.GetPooledObject("VfxFlash");
            muzzle.transform.position = gameObject.transform.position;
            muzzle.SetActive(true);

        }
        catch (System.Exception)
        {
            //throw;
        }
        _aliveTime = 20;
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
            aliveTime = 20;//_aliveTime;
            gameObject.SetActive(false);
        }

    }

    private void HitFx()
    {
        hit = ObjectPooler.SharedInstance.GetPooledObject("VfxImpact");
        hit.transform.position = transform.position;
        hit.transform.rotation = transform.rotation;

        hit.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy")
        {
            HitFx();
            enemyTriggered = other.gameObject;
            Enemy _enemy = enemyTriggered.GetComponent<Enemy>();
            _enemy.removeHealth(damage);
            gameObject.SetActive(false);            
        }

    }

}
