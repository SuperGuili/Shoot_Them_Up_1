using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public GameObject[] enemy;
    private GameObject player;
    private Vector3 spawnPosition;

    private int wave = 1;
    public float nextWave = 10;
    private float wavecounter = 0;
    public float difficulty = 1;
    public bool resetAll = false;

    private bool waveEnable = false;
    private float x = 0, y = 59.5f, z = 400; // Initial Spawn position
    private int enemyGap = 25;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update    
    void Start()
    {

    }

    void Update()
    {
        if (player == null)
        {
            OnEnable();
            return;
        }

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
    private string position = "center";
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

        for (int i = 0; i < wave; i++)
        {
            //GameObject go = Instantiate(enemy[0]) as GameObject;
            GameObject go = ObjectPooler.SharedInstance.GetPooledObject("Enemy");
            if (i == 0) // ok -- center
            {
                go.transform.position = spawnPosition;
                go.SetActive(true);
            }
            else if (i % 2 == 1) // Odd -- ok
            {
                spawnPosition.x -= enemyGap * i; // set the gap for v formation
                spawnPosition.z += 30;
                go.transform.position = spawnPosition;
                go.SetActive(true);
            }
            else if (i % 2 == 0) // Even -- ok
            {
                spawnPosition.x += enemyGap * i; // set the gap for v formation
                go.transform.position = spawnPosition;
                go.SetActive(true);
            }
        }
    }

    public void ResetAll()
    {
        wave = 1;
        nextWave = 10;
        wavecounter = 0;
        difficulty = 1;
        waveEnable = false;
        x = 0; y = 59.5f; z = 400;
        enemyGap = 25;
    }
}
