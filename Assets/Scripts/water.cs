using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class water : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Enemy")
        {
            GameObject go = ObjectPooler.SharedInstance.GetPooledObject("VfxSplash");
            go.transform.position = other.transform.position;
            go.transform.rotation = transform.rotation;
            go.transform.SetParent(transform);
            go.SetActive(true);
        }
    }
}
