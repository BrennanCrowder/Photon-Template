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
    private bool grabbing = false;
    private Coroutine moveRoutine;
    private Coroutine grabRoutine;
    public BTriggerScript grabTrigger;
    public BCrushScript lCrushTrigger;
    public BCrushScript rCrushTrigger;
    public GameObject rHand;
    public GameObject lHand;
    public float grabDistance = 1;

    private GameObject grabbedObject;
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
        if (ctx.canceled)
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
        var scrpt = grabbedObject.GetComponent<SPlayerControls>();
        lHand.GetComponent<MoveToTarget>().ResetSpeed();
        rHand.GetComponent<MoveToTarget>().ResetSpeed();
        grabbing = false;
        scrpt.Dropped();// playerBody.useGravity = false;
        scrpt.playerBody.GetComponent<Collider>().enabled = true;
        grabbedObject.transform.SetParent(null);
    }

    public void Throw(InputAction.CallbackContext ctx)
    {

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
        SPlayerControls rbScript = sPlayer.GetComponent<SPlayerControls>();
        Rigidbody rb = rbScript.playerBody;
        Collider c= sPlayer.GetComponent<Collider>();
        GameObject hand = Vector3.Distance(rHand.transform.position, sPlayer.transform.position) > Vector3.Distance(lHand.transform.position, sPlayer.transform.position) ? lHand : rHand;
        MoveToTarget scrpt = hand.GetComponent<MoveToTarget>();
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
            rb.transform.position = hand.transform.position;
            rbScript.Grabbed();
            sPlayer.transform.SetParent(hand.transform);
            scrpt.ResetTarget();
            grabbedObject = sPlayer;
        }
        
        yield return null;
    }
}
