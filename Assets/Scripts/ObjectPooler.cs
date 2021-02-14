using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand;
}
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;

    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    void Awake()
    {
        SharedInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();

        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                if (obj.tag == "VfxBullet" || obj.tag == "BulletEnemy")
                {
                    Transform Bullets = GameObject.FindGameObjectWithTag("Bullets").transform;
                    obj.transform.SetParent(Bullets);
                }
                if (obj.tag == "VfxImpact" || obj.tag == "VfxFlash" || obj.tag == "VfxMuzzle" || obj.tag == "VfxFirePlayer"
                     || obj.tag == "VfxFire" || obj.tag == "VfxImpactEnemy")
                {
                    Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
                    obj.transform.SetParent(VFX);
                }
                if (obj.tag == "Terrain")
                {
                    Transform TerrainManager = GameObject.FindGameObjectWithTag("TerrainManager").transform;
                    obj.transform.SetParent(TerrainManager);
                }
                if (obj.tag == "Enemy")
                {
                    Transform Enemies = GameObject.FindGameObjectWithTag("Enemies").transform;
                    obj.transform.SetParent(Enemies);
                }
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public GameObject GetPooledObject(string tag)
    {
        //1
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //2
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
}
