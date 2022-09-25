using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerRoll : MonoBehaviour
{
    private Rigidbody playerBody;

    void Start()
    {
        playerBody = GetComponentInParent<BPlayerControls>().playerBody;
    }

    void Update()
    {
        
    }
}
