using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt; // player position
    private Vector3 startOffset; // camera offset
    private Vector3 moveVector; // used to store position

    public float transition = 0.0f; // used to count the transition time
    private float animationDuration = 3.0f; // transition duration
    private Vector3 animationOffset = new Vector3(0, 5, 5); // Camera offset for the transition

    void Start()
    {
        try
        {
            lookAt = GameObject.FindGameObjectWithTag("Player").transform;
            startOffset = transform.position - lookAt.position;
        }
        catch (System.Exception)
        {
            //throw;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAt == null)
        {
            Start();
            return;
        }

        moveVector = lookAt.position + startOffset;

        if (transition > 1.0f)
        {
            transform.position = moveVector;
            transform.LookAt(lookAt.position + new Vector3(0, 5, 0));           
        }
        else
        {
            //Camera animation at the start
            transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(lookAt.position + new Vector3(0, 5, 0));
        }
    }
}
