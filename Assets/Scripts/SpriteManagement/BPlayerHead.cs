using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerHead : MonoBehaviour
{
    public Animator headAnimator;
    private Rigidbody playerBody;
    private BPlayerControls playerControls;

    void Start()
    {
        playerControls = GetComponentInParent<BPlayerControls>();
        playerBody = playerControls.playerBody;
    }

    void Update()
    {
        headAnimator.SetBool("Moving", playerBody.velocity.magnitude > 0.25f || playerControls.isHolding);
    }
}
