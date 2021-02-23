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
    private float _aliveTime = 10;

    public GameObject hit = null;
    private bool soundFx = true;
    private bool hitPosition = false;

    private AudioSource audioSource;

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
        bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
        soundFx = true;
        hitPosition = false;
        audioSource = GetComponent<AudioSource>();
        gameObject.GetComponentInChildren<TrailRenderer>().Clear();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsOver)
        {
            aliveTime = _aliveTime;
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
            aliveTime = _aliveTime;
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
        ParticleSystem scale;
        for (int i = 0; i < 3; i++)
        {
            scale = hit.transform.GetChild(i).GetComponent<ParticleSystem>();
            scale.transform.localScale /= 3f;
        }
        hitPosition = true;
        hit.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            HitFx();
            enemyTriggered = other.gameObject;
            Player _player = enemyTriggered.GetComponent<Player>();
            _player.RemoveHealth(damage);
            gameObject.SetActive(false);
        }
    }
}
