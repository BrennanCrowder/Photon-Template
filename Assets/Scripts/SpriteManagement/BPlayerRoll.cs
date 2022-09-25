using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerRoll : MonoBehaviour
{
    public float maxRotation = 45f;
    public SpriteRenderer bodySprite;
    private Rigidbody playerBody;

    void Start()
    {
        playerBody = GetComponentInParent<BPlayerControls>().playerBody;
    }

    void Update()
    {
        Vector3 rotate = Vector3.zero;
        rotate.z = maxRotation * -(playerBody.velocity.x / (playerBody.velocity.x + 2));
    }
}
