using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private GameObject player;
    private float spawnZ = 0;
    private float terrainLenght = 1000;
    private int maxSpawnedTerrains = 3;

    public List<GameObject> activeTerrains;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        catch (System.Exception)
        {

        }
        activeTerrains = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            Start();
            return;
        }

        if (activeTerrains.Count < maxSpawnedTerrains && player.activeInHierarchy)
        {
            Spawnterrain();
        }

        if (!player.activeInHierarchy)
        {
            for (int i = 0; i < activeTerrains.Count; i++)
            {
                GameObject go = activeTerrains[i];
                go.SetActive(false);
            }
        }
        if (player.activeInHierarchy)
        {
            for (int i = 0; i < activeTerrains.Count; i++)
            {
                GameObject go = activeTerrains[i];
                go.SetActive(true);
            }
        }
        if (!player.GetComponent<Player>().isAlive)
        {
            for (int i = 0; i < activeTerrains.Count; i++)
            {
                GameObject go = activeTerrains[i];
                go.SetActive(false);                
            }
        }

        try
        {
            if (player.transform.position.z + terrainLenght > spawnZ && activeTerrains.Count >= maxSpawnedTerrains)
            {
                DeactivateTerrain();
            }
        }
        catch (System.Exception)
        {

        }
    }

    private void Spawnterrain()
    {
        Vector3 moveVector = new Vector3(-500, 0, (spawnZ));
        spawnZ += terrainLenght;
        GameObject go = ObjectPooler.SharedInstance.GetPooledObject("Terrain");
        go.transform.position = moveVector;
        activeTerrains.Add(go);
        go.SetActive(true);
    }

    private void DeactivateTerrain()
    {
        GameObject go = activeTerrains[0];
        go.SetActive(false);
        activeTerrains.RemoveAt(0);
    }

    public void ResetAll()
    {
        for (int i = 0; i < activeTerrains.Count; i++)
        {
            GameObject go = activeTerrains[i];
            go.SetActive(false);
            activeTerrains.Remove(go);
        }
        spawnZ = 0;
        terrainLenght = 1000;
        maxSpawnedTerrains = 3;
        activeTerrains = new List<GameObject>();
    }

    private void OnDisable()
    {
        ResetAll();
    }
}
