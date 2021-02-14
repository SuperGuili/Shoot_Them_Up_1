using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            playerTransform = GameObject.Find("Player").GetComponent<Player>().transform;
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
            return;
        }
        if (transform.position.z < playerTransform.position.z -1000)////
        {
            Delete();
        }
    }
    public void Delete()
    {
        gameObject.SetActive(false);
    }

}
