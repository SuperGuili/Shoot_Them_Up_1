using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIntro : MonoBehaviour
{
    public float intro_duration = 6;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitForIntro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator waitForIntro()
    {
        yield return new WaitForSeconds(intro_duration);

        SceneManager.LoadScene(1);
    }
}
