using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Transform targetTransform;
    public Transform defaultTarget;
    private float defaultSpeed;
    public float speed = 0.001f;
    public bool pauseReposition = false;
    private void Awake()
    {
        defaultTarget = targetTransform;
        defaultSpeed = speed;
    }
    private void FixedUpdate()
    {
        if (!pauseReposition)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, speed);
        }
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    public void ResetTarget()
    {
        targetTransform = defaultTarget;
    }

    public void setSpeed(float v)
    {
        speed = v;
    }

    public void ResetSpeed()
    {
        speed = defaultSpeed;
    }

    public void ResetAll()
    {
        targetTransform = defaultTarget;
        speed = defaultSpeed;
        pauseReposition = false;
    }
}
