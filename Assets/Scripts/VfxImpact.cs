using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxImpact : MonoBehaviour
{
    public float aliveTime = 3;

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
            /// Impact Vfx Deactivate and reset after use
            aliveTime = 3;
            Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
            gameObject.transform.SetParent(VFX);
            gameObject.SetActive(false);
        }
    }
}
