using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float aliveTime; // used to count and disable the bullet object
    public int damage; // Bullet damage 
    public float movSpeed; // velocity of bullet

    private Transform playerTransform; // player object
    private GameObject enemyTriggered; // enemy object
    private Rigidbody bullet; // bullet rigidbody
    private float playerSpeed; // get the player speed

    public GameObject muzzle; // Muzzle flash object

    // Start is called before the first frame update
    private void OnEnable()
    {
        try
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            playerSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().speed;

            bullet = GetComponent<Rigidbody>();
            bullet.velocity = transform.up * (movSpeed + playerSpeed);

            /// Muzzle Flash
            muzzle = ObjectPooler.SharedInstance.GetPooledObject("VfxFlash");
            muzzle.transform.SetParent(playerTransform);
            muzzle.transform.position = gameObject.transform.position;
            muzzle.SetActive(true);            
        }
        catch (System.Exception)
        {
            //throw;
        }
        gameObject.GetComponentInChildren<TrailRenderer>().Clear(); // clear the bullet trail fx
        aliveTime = 5;
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

        aliveTime -= 1 * Time.deltaTime;
        if (aliveTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {          
            enemyTriggered = other.gameObject;
            enemyTriggered.GetComponent<Enemy>().RemoveHealth(damage);
            gameObject.SetActive(false);            
        }
    }
}
