using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BPlayerControls : MonoBehaviour
{
    public float speed = .001f;
    public Rigidbody playerBody;
    private InputActionMap playerControls;
    private bool moving;
    private bool throwing = false;
    private Coroutine moveRountine;
    public BTriggerScript grabTrigger;
    public BCrushScript lCrushTrigger;
    public BCrushScript rCrushTrigger;

    private void Awake()
    {
        playerControls = new InputActionMap("PlayerControls");
        
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void FixedUpdate()
    {
       if (playerBody.velocity.x > .25)
       {
            lCrushTrigger.enableCrush = true;
       } else
        {

        }
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moving = false;
            //crushTrigger.enableCrush = true;
            StopCoroutine(moveRountine);
        }
        if (ctx.performed && !throwing)
        {
            moving = true;
            Debug.Log("Moving...");
            moveRountine = StartCoroutine(Moving(ctx));
        }
        
    }

    public void Grab(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (AttemptGrab())
            {
                Debug.Log("Grab Success");
            }
            else Debug.Log("Grab Fail");
        }
    }

    public bool AttemptGrab()
    {
        GameObject splayer = grabTrigger.GetBody();
        if (splayer)
        {
            return true;
        }
        return false;
    }

    IEnumerator Moving(InputAction.CallbackContext ctx)
    {
        float time = Time.time;
        while (moving)
        {
            //crushTrigger.enableCrush = true;
            playerBody.AddForce(new Vector3(speed * ctx.ReadValue<Vector2>().x, 0, 0), ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
