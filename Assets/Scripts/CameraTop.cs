using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTop : MonoBehaviour
{
    private Transform lookAt;
    private Vector3 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        lookAt = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = lookAt.position;
        moveVector.x -= 0; //
        moveVector.y += 40; //z
        moveVector.z -= 40; //camera angle
        
        transform.position = moveVector;
        transform.LookAt(lookAt.position + new Vector3(0, 0, 80));//move the camera
    }
}
