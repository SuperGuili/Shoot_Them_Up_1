using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxDust : MonoBehaviour
{
    public float aliveTime = 20;
    private Transform VFX;
    private void OnEnable()
    {
        aliveTime = 20;
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime -= 1 * Time.deltaTime;

        if (aliveTime <= 0)
        {
            /// Smoke Vfx Deactivate after use
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {              

    }
}
