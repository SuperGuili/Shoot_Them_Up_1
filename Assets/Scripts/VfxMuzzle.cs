using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxMuzzle : MonoBehaviour
{
    public float aliveTime = 2;

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
            /// Muzzle Flash Deactivate after use
            aliveTime = 2;
            Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
            gameObject.transform.SetParent(VFX);
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {

    }
}
