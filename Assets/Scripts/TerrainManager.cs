using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private Transform playerTransform;
    private float spawnZ = -500.0f;
    private float terrainLenght = 500;
    private int maxSpawnedTerrains = 5;

    private int lastPrefabIndex = 0;

    private float safeZone = 500;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            for (int i = 0; i < maxSpawnedTerrains; i++)
            {
                if (i < 2)
                    Spawnterrain();
                else
                    Spawnterrain();
            }
        }
        catch (System.Exception)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            Start();
        }

        try
        {
            if (playerTransform.position.z - safeZone > (spawnZ - maxSpawnedTerrains * terrainLenght) && playerTransform != null)
            {
                Spawnterrain();
            }
        }
        catch (System.Exception)
        {

        }

    }

    private void Spawnterrain()
    {
        Vector3 moveVector = new Vector3(-500, 0, (1 * spawnZ));
        spawnZ += terrainLenght;
        GameObject go = ObjectPooler.SharedInstance.GetPooledObject("Terrain");
        go.transform.position = moveVector;
        go.SetActive(true);
    }

    public void ResetAll()
    {
        spawnZ = -500.0f;
        terrainLenght = 500;
        maxSpawnedTerrains = 5;
        lastPrefabIndex = 0;
        safeZone = 500;
    }
}
