using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    private GameObject player;
    private Vector3 spawnPosition;

    private int wave = 1; // Number of enemies per wave
    public float nextWave = 10; // Seconds to next wave of enemies
    private float wavecounter = 0; // counts the seconds

    private bool waveEnable = false; // enables the wave to call spawnenemies();
    private float x = 0, y = 59.5f, z = 400; // Initial Spawn position
    private int enemyGap = 25; // gap of the V formation
    private string position = "center"; // Initial position of enemy in the formation

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //position = "center";
    }

    // Start is called before the first frame update    
    void Start()
    {

    }
    void Update()
    {
        //Get the player in case its null
        if (player == null)
        {
            OnEnable();
            return;
        }

        //Check if the player is alive before spawning enemies
        if (player.gameObject.activeInHierarchy)
        {
            if (waveEnable == true)
            {
                SpawnEnemies();
                if (wave < 10)
                {
                    wave++;
                }
                waveEnable = false;
            }

            wavecounter += Time.deltaTime;
            if (wavecounter > nextWave)
            {
                nextWave += 10;
                waveEnable = true;
            }
        }
    }

    // Function to spawn enemies
    private void SpawnEnemies()
    {
        //Left or right wave position
        switch (position)
        {
            case "center":
                x = 0;
                position = "right";
                break;
            case "right":
                x = 130;
                position = "center2";
                break;
            case "center2":
                x = 0;
                position = "left";
                break;
            case "left":
                x = -130;
                position = "center";
                break;
        }
        z = (int)player.transform.position.z + 500;
        spawnPosition = new Vector3(x, y, z);

        //Loop to spawn in V formation
        for (int i = 0; i < wave; i++)
        {
            GameObject go = ObjectPooler.SharedInstance.GetPooledObject("Enemy");
            if (i == 0) // center - first enemy
            {
                go.transform.position = spawnPosition;
                go.SetActive(true);
            }
            else if (i % 2 == 1) // Odd 
            {
                spawnPosition.x -= enemyGap * i; // set the gap for v formation
                spawnPosition.z += 30;
                go.transform.position = spawnPosition;
                go.SetActive(true);
            }
            else if (i % 2 == 0) // Even 
            {
                spawnPosition.x += enemyGap * i; // set the gap for v formation
                go.transform.position = spawnPosition;
                go.SetActive(true);
            }
        }
    }

    // Function to reset the values when the game is over
    public void ResetAll()
    {
        wave = 1;
        nextWave = 10;
        wavecounter = 0;
        waveEnable = false;
        x = 0; y = 59.5f; z = 400;
        enemyGap = 25;
        position = "center";
    }
}
