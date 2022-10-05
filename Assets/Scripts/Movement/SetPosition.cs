using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour
{
    public Transform location;

    private void Awake()
    {
        transform.position = location.position;
    }

    public void ChangePosition(Vector3 position)
    {
        transform.position = position;
    }
}

