using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Transform targetTransform;
    public float speed = .5f;
    public bool pauseReposition = false;
    private void Update()
    {
        if (!pauseReposition)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, speed);
        }
    }
}
