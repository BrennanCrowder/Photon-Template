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
        if (playerControls.grabbing && playerControls.grabHand == gameObject)
        {
            if (playerControls.grabbedObject != null)
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
        else
        {
            handSprite.sprite = closedHand;
            fingers.SetActive(false);
        }
    }
}
