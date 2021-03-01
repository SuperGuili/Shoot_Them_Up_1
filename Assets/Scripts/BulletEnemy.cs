using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float aliveTime = 10; // used to count and disable the bullet object
    public int damage = 1; // Bullet damage
    public float movSpeed = 50; // velocity of bullet

    private GameObject playerTriggered; // player object for collision
    private GameObject player; // player object
    private Rigidbody bullet; // bullet rigidbody

    public GameObject hit = null; // hit fx
    private bool soundFx = true; // hit sound fx checker
    private bool hitPosition = false; // used to check collision position

    private AudioSource audioSource; //

    // Start is called before the first frame update
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");        
        bullet = GetComponent<Rigidbody>();
        bullet.velocity = transform.up * (movSpeed);
        bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
        soundFx = true;
        hitPosition = false;
        audioSource = GetComponent<AudioSource>();
        gameObject.GetComponentInChildren<TrailRenderer>().Clear();
        aliveTime = 10;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsOver)
        {
            gameObject.SetActive(false);
        }

        if (hitPosition)
        {
            hit.transform.position = transform.position;
            hitPosition = false;
        }

        try
        {       
            if (!player.activeSelf && transform.position.z < player.transform.position.z + 100)
            {
                gameObject.SetActive(false);
            }
        }
        catch (System.Exception)
        {
           
        }

        aliveTime -= 1 * Time.deltaTime;
        if (aliveTime <= 0)
        {
            gameObject.SetActive(false);
        }

        if (player != null && gameObject.activeInHierarchy)
        {
            if (transform.position.z - player.transform.position.z <= 10 && soundFx)
            {
                audioSource.PlayOneShot(audioSource.clip, 1);
                soundFx = false;
            }
        }
    }

    private void HitFx()
    {
        hit = ObjectPooler.SharedInstance.GetPooledObject("VfxImpact");
        hit.transform.SetParent(player.transform);
        hit.transform.position = transform.position;
        hit.transform.rotation = transform.rotation;
        hitPosition = true;
        hit.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HitFx();
            playerTriggered = other.gameObject;
            playerTriggered.GetComponent<Player>().RemoveHealth(damage);
            gameObject.SetActive(false);
        }
    }
}
