using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerHands : MonoBehaviour
{
    public Sprite closedHand;
    public Sprite openHand;
    public Sprite grabHand;
    public GameObject fingers;
    public SpriteRenderer handSprite;

    private BPlayerControls playerControls;

    void Start()
    {
        playerControls = GetComponentInParent<BPlayerControls>();
    }

    void Update()
    {
        /*if (playerControls.isGrabbing && playerControls.grabHand == gameObject)
        {
            if (playerControls.isHolding)
            {
                handSprite.sprite = grabHand;
                fingers.SetActive(true);
            }
            else
            {
                handSprite.sprite = openHand;
                fingers.SetActive(false);
            }
        }
        else if (!playerControls.isHolding)
        {
            handSprite.sprite = closedHand;
            fingers.SetActive(false);
        }*/
    }

    public void enableGrabbingSprite()
    {
        handSprite.sprite = openHand;
        fingers.SetActive(false);
    }

    public void enableHoldSprite()
    {
        handSprite.sprite = grabHand;
        fingers.SetActive(true);
    }

    public void enableClosedHand()
    {
        handSprite.sprite = closedHand;
        fingers.SetActive(false);
    }
}
