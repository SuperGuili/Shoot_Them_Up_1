using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxSplash : MonoBehaviour
{
    public float aliveTime = 3;

    private void OnEnable()
    {
        aliveTime = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsOver)
        {
            aliveTime = -1;
        }

        aliveTime -= 1 * Time.deltaTime;
        if (aliveTime <= 0)
        {
            aliveTime = 3;
            /// Splash Vfx Deactivate and reset after use            
            Transform VFX = GameObject.FindGameObjectWithTag("VFX").transform;
            gameObject.transform.SetParent(VFX);
            gameObject.SetActive(false);
        }
    }
}
