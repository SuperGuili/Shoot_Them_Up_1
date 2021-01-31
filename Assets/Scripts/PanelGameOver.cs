using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGameOver : MonoBehaviour
{
    public Text scoreText;
    
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DeathMenuOn(int score)
    {
        scoreText.text = score.ToString();
        gameObject.SetActive(true);
    }
}

