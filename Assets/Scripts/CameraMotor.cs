using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    private Vector3 startOffset;
    private Vector3 moveVector;

    public float transition = 0.0f;
    private float animationDuration = 3.0f;
    private Vector3 animationOffset = new Vector3(0, 5, 5);

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

        if (transition > 1.0f)
        {
            moveVector = lookAt.position + startOffset;
            transform.position = moveVector;
            transform.LookAt(lookAt.position + new Vector3(0, 5, 0));           
        }
        else
        {
            moveVector = lookAt.position + startOffset;
            //Camera animation at the start
            transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(lookAt.position + new Vector3(0, 5, 0));
        }

        //reset transition
    }
}
