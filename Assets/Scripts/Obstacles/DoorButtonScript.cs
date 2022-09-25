using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorButtonScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite pressedSprite;
    public Sprite unPressedSprite;
    public UnityEvent pressed;
    public UnityEvent unpressed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = unPressedSprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SPlayer" || other.tag == "BPlayer")
        {
            pressed.Invoke();
            spriteRenderer.sprite = pressedSprite;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SPlayer" || other.tag == "BPlayer")
        {
            unpressed.Invoke();
            spriteRenderer.sprite = unPressedSprite;
        }
    }
}
