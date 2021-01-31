using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float aliveTime = 10;
    public int damage;
    public float movSpeed;

    private GameObject enemyTriggered;
    GameObject player;
    private Rigidbody bullet;
    private float playerSpeed;
    private float _aliveTime = 15;

    // Start is called before the first frame update
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerSpeed = player.GetComponent<Player>().speed;
        }
        bullet = GetComponent<Rigidbody>();
        bullet.velocity = transform.up * (movSpeed + playerSpeed);

        //_aliveTime = aliveTime;
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
            aliveTime = _aliveTime;
            gameObject.SetActive(false);
        }

        if (!player.gameObject.GetComponent<Player>().isAlive)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            enemyTriggered = other.gameObject;
            Player _player = enemyTriggered.GetComponent<Player>();
            _player.removeHealth(damage);
            gameObject.SetActive(false);
        }
    }
}
