using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxDust : MonoBehaviour
{
    public float aliveTime = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime -= 1 * Time.deltaTime;
        if (aliveTime <= 0)
        {
            /// Smoke Vfx Deactivate and reset after use
            aliveTime = 20;
            Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
            gameObject.transform.SetParent(VFX);
            gameObject.SetActive(false);
        }
    }
}
