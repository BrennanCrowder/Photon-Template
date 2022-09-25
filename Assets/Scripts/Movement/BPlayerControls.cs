using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BPlayerControls : MonoBehaviour
{
    public float moveSpeed = .001f;
    public Rigidbody playerBody;
    private InputActionMap playerControls;
    private bool moving;
    private bool throwing = false;
    [HideInInspector] public bool grabbing = false;
    private Coroutine moveRoutine;
    private Coroutine grabRoutine;
    private Coroutine dropRoutine;
    private Coroutine throwRoutine;
    private float escapeTimer;
    public BTriggerScript grabTrigger;
    public BCrushScript lCrushTrigger;
    public BCrushScript rCrushTrigger;
    public GameObject Head;
    public GameObject rHand;
    public GameObject lHand;
    public GameObject crosshair;
    public Transform dropLocation;
    public float grabDistance = 1;
    private bool fire;
    [HideInInspector] public GameObject grabbedObject;
    [HideInInspector] public GameObject grabHand;
    public float grabSpeed = .75f;

    private WaitForFixedUpdate fixedWait;
    private void Awake()
    {
        playerControls = new InputActionMap("PlayerControls");
        fixedWait = new WaitForFixedUpdate();
        
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
        lCrushTrigger.enableCrush = playerBody.velocity.x > .15;
        rCrushTrigger.enableCrush = playerBody.velocity.x < -.15;
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && moving)
        {
            moving = false;
            StopCoroutine(moveRoutine);
        }
        if (ctx.performed && !throwing && !grabbing)
        {
            moving = true;
            Debug.Log("Moving...");
            moveRoutine = StartCoroutine(Moving(ctx));
        }
        
    }

    public void Grab(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!grabbing) 
            {
                Debug.Log("Grabbing...");
                AttemptGrab();
            } else
            {
                Debug.Log("Dropping...");
                Drop();
            }
        } 
    }

    private void AttemptGrab()
    {
        GameObject sPlayer = grabTrigger.GetBody();
        if (sPlayer)
        {
            grabRoutine = StartCoroutine(Grab(sPlayer));
        }
    }

    private void Drop()
    {
        dropRoutine = StartCoroutine(Dropping());
        
    }

    public void Aim(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && grabbing && !throwing)
        {
            throwRoutine = StartCoroutine(Throw(ctx));
        } else if (ctx.canceled && throwing)
        {
            //StopCoroutine(throwRoutine);
            throwing = false;
        }
    }


    public void CursorInfo(InputAction.CallbackContext ctx)
    {
        EyesAt(ctx);
        if (throwing)
        {
            
            crosshair.transform.position = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            crosshair.transform.position = new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 0);
        }
    }

    public void Fire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !fire)
        {
            fire = true;
        }
    }

    private void EyesAt(InputAction.CallbackContext ctx)
    {
        //TBD
    }

    public int Escape(int count)
    {
        var hand = grabbedObject.transform.parent.gameObject;
        if (escapeTimer == 0)
        {
            escapeTimer = Time.time;
        }
        else if (Time.time - escapeTimer >= .5f)
        {
            escapeTimer = 0;
            count = 1;
        }
        switch (count)
        {
            case (3):
                hand.transform.Translate(new Vector3(hand.transform.position.x * .25f, .5f, 0) * count, Space.World);
                var handScript = hand.GetComponent<MoveToTarget>();
                var scrpt = grabbedObject.GetComponent<SPlayerControls>();

                playerBody.angularDrag = 0.05f;
                scrpt.transform.SetParent(null);
                scrpt.Dropped();

                grabbing = false;
                handScript.ResetTarget();
                handScript.ResetSpeed();
                grabbedObject = null;
                escapeTimer = 0;
                break;

            default:
                hand.transform.Translate(new Vector3( hand.transform.position.x * .25f, .5f, 0) * count, Space.World);
                break;
        }
        count++;
        return count;
    }


    IEnumerator Moving(InputAction.CallbackContext ctx)
    {
        float time = Time.time;
        while (moving)
        {
            playerBody.AddForce(new Vector3(moveSpeed * ctx.ReadValue<Vector2>().x, 0, 0), ForceMode.Acceleration);
            yield return fixedWait;
        }
        yield return null;
    }

    IEnumerator Grab(GameObject sPlayer)
    {
        grabbing = true;
        Debug.Log(sPlayer);
        SPlayerControls rbScript = sPlayer.GetComponent<SPlayerControls>();// SPlayer
        Rigidbody rb = rbScript.playerBody; // SBody

        GameObject hand = Vector3.Distance(rHand.transform.position, rb.transform.position) > Vector3.Distance(lHand.transform.position, rb.transform.position) ? lHand : rHand; // Closest Hand
        grabHand = hand;

        MoveToTarget scrpt = hand.GetComponent<MoveToTarget>(); // Hand Script

        scrpt.SetTarget(rb.transform);
        scrpt.setSpeed(grabSpeed);
        
        while (grabbing == true && Vector3.Distance(hand.transform.position,rb.transform.position) >= grabDistance)
        {
            if (grabTrigger.GetBody() == null)
            {
                scrpt.ResetTarget();
                grabbing = false;
            }
            yield return fixedWait;
        }
        if (grabbing == true)
        {
            //grabTrigger.ResetBody();
            playerBody.angularDrag = 10;
            lCrushTrigger.enableCrush = false;
            rCrushTrigger.enableCrush = false;
            //rb.transform.rotation = hand.transform.rotation;
            rbScript.Grabbed();
            sPlayer.transform.SetParent(hand.transform);
            rb.transform.position = hand.transform.position;
            scrpt.ResetTarget();
            grabbedObject = sPlayer;
        } else
        {
            Debug.Log("GRABBING FAILED");
        }
        
        yield return null;
    }

    IEnumerator Dropping()
    {
        GameObject hand = grabbedObject.transform.parent.gameObject;
        var handScript = hand.GetComponent<MoveToTarget>();
        var scrpt = grabbedObject.GetComponent<SPlayerControls>();

        handScript.SetTarget(dropLocation);

        while(Vector3.Distance(hand.transform.position, dropLocation.position) >= .3)
        {
            yield return fixedWait;
        }
        
        playerBody.angularDrag = 0.05f;
        //scrpt.transform.SetParent(null);
        scrpt.Dropped();
       
        grabbing = false;
        handScript.ResetTarget();
        handScript.ResetSpeed();
        grabbedObject = null;
        escapeTimer = 0;
        yield return null;
    }

    IEnumerator Throw(InputAction.CallbackContext ctx)
    {
        throwing = true; 
        var hand = grabbedObject.transform.parent.gameObject;
        var handScript = hand.GetComponent<MoveToTarget>();
        var sPlayerScript = grabbedObject.GetComponent<SPlayerControls>();
        handScript.pauseReposition = true;
        Vector3 relativePos = Vector3.zero;
        Vector3 changePos = Vector3.zero;
        Vector3 origHandPos = handScript.defaultTarget.transform.position;
        while (throwing && grabbing && !fire)
        {
            relativePos = crosshair.transform.position - hand.transform.position;
            changePos = (relativePos.magnitude / (relativePos.magnitude + 2)) * relativePos.normalized * 0.75f;
            hand.transform.position = origHandPos - changePos;
          
            yield return fixedWait;
        }
        if (fire)
        {
            sPlayerScript.playerBody.AddForce(changePos * 50, ForceMode.Impulse);
            sPlayerScript.ThrowAnim();
        }
        handScript.pauseReposition = false;
        fire = false;
    yield return null;
    }
}
