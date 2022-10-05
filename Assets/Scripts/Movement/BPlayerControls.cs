using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BPlayerControls : MonoBehaviour
{
    public float moveSpeed = .001f;
    public float grabDistance = 1;
    public float grabSpeed = .75f;
    public Rigidbody playerBody;
    public GameObject Head;
    public GameObject rHand;
    public GameObject lHand;
    private InputActionMap playerControls;
    public BTriggerScript grabTrigger;
    public BCrushScript lCrushTrigger;
    public BCrushScript rCrushTrigger;
    public GameObject crosshair;
    public Transform dropLocation;

    private ParticleSystem deathParticles;
    private Transform startingPos;
    private WaitForFixedUpdate fixedWait;
    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    private Coroutine moveRoutine;
    private Coroutine grabRoutine;
    private Coroutine dropRoutine;
    private Coroutine throwRoutine;
    

    
    
    
    
    
    

    public bool isHolding;
    public bool isMoving;
    public bool isThrowing;
    public bool fire;
    public bool isDropping;
    public bool isGrabbing;
    private float escapeTimer;
    public GameObject grabbedObject;
    public GameObject grabHand;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        deathParticles = playerBody.GetComponent<ParticleSystem>();
        foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
        {
            renderers.Add(rend);
        }
        startingPos = transform;
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
        if (!isHolding && !isGrabbing)
        {
            lCrushTrigger.enableCrush = playerBody.velocity.x > .15;
            rCrushTrigger.enableCrush = playerBody.velocity.x < -.15;
        } else if (isHolding && grabbedObject.GetComponent<SPlayerControls>().isDead) 
        {
            /*holding = false;
            isGrabbing = false;
            playerBody.angularDrag = 0.05f;
            grabbedObject = null;
            grabHand = null;
            escapeTimer = 0;*/
            ResetActions();
        }
        
    }

    public void ResetActions()
    {
        //Debug.Log("Resetting Actions");
        if (grabbedObject)
        {
            grabbedObject.transform.SetParent(null);
        }
        isHolding = false;
        isMoving = false;
        isThrowing = false;
        fire = false;
        isDropping = false;
        isGrabbing = false;
        escapeTimer = 0;
        grabHand = null;
        grabbedObject = null;
        playerBody.angularDrag = 0.05f;
        lHand.GetComponent<BPlayerHands>().enableClosedHand();
        lHand.GetComponent<MoveToTarget>().ResetAll();

        rHand.GetComponent<BPlayerHands>().enableClosedHand();
        rHand.GetComponent<MoveToTarget>().ResetAll();
        StopAllCoroutines();
}

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled && isMoving)
        {
            isMoving = false;
            StopCoroutine(moveRoutine);
        }
        if (ctx.performed && !isHolding)
        {
            isMoving = true;
            moveRoutine = StartCoroutine(Moving(ctx));
        }
        
    }

    public void Grab(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!isHolding && !isThrowing && !isDropping && !isGrabbing) 
            {
                //Debug.Log("Grabbing...");
                AttemptGrab();
            } else if (isHolding && !isThrowing && !isDropping && !isGrabbing) ///
            {
                //Debug.Log("Dropping...");
                dropRoutine = StartCoroutine(Dropping());
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

    public void Aim(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isHolding && !isThrowing && !isGrabbing)
        {
            throwRoutine = StartCoroutine(Throw(ctx));
        } else if (ctx.canceled && isThrowing)
        {
            isThrowing = false;
        }
    }


    public void CursorInfo(InputAction.CallbackContext ctx)
    {
        EyesAt(ctx);
        if (isThrowing)
        {
            
            crosshair.transform.position = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            crosshair.transform.position = new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 0);
        }
    }

    public void Fire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isThrowing && !fire)
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
                hand.transform.Translate(new Vector3(0, .5f, 0) * count, Space.World);
                var handScript = hand.GetComponent<MoveToTarget>();
                var scrpt = grabbedObject.GetComponent<SPlayerControls>();

                scrpt.Dropped(true, true);

                //playerBody.angularDrag = 0.05f;
                //isDropping = false;
                //isHolding = false;
                //handScript.ResetTarget();
                //handScript.ResetSpeed();
                //grabbedObject = null;
                //escapeTimer = 0;
                ResetActions();
                break;

            default:
                hand.transform.Translate(new Vector3(0, .5f, 0) * count, Space.World);
                break;
        }
        count++;
        return count;
    }


    IEnumerator Moving(InputAction.CallbackContext ctx)
    {
        //float time = Time.time;
        while (isMoving)
        {
            playerBody.AddForce(new Vector3(moveSpeed * ctx.ReadValue<Vector2>().x, 0, 0), ForceMode.Acceleration);
            yield return fixedWait;
        }
        yield return null;
    }

    IEnumerator Grab(GameObject sPlayer)
    {
        isGrabbing = true;

        SPlayerControls rbScript = sPlayer.GetComponent<SPlayerControls>();// SPlayer
        if (rbScript.isDead)
        {
            isGrabbing = false;
            yield return null;
        }
        Rigidbody rb = rbScript.playerBody; // SBody

        //lCrushTrigger.enableCrush = false;
        //rCrushTrigger.enableCrush = false;

        GameObject hand = Vector3.Distance(rHand.transform.position, rb.transform.position) > Vector3.Distance(lHand.transform.position, rb.transform.position) ? lHand : rHand; // Closest Hand
        grabHand = hand;
        
        hand.GetComponent<BPlayerHands>().enableGrabbingSprite();

        MoveToTarget scrpt = hand.GetComponent<MoveToTarget>(); // Hand Script

        scrpt.SetTarget(rb.transform);
        scrpt.setSpeed(grabSpeed);

        while (isGrabbing && Vector3.Distance(hand.transform.position, rb.transform.position) >= grabDistance)
        {
            if (grabTrigger.GetBody() == null || rbScript.isDead)
            {
                scrpt.ResetTarget();
                isGrabbing = false;
            }
            yield return fixedWait;
        }
        if (isGrabbing == true)
        {
            playerBody.angularDrag = 10;
            lCrushTrigger.enableCrush = false;
            rCrushTrigger.enableCrush = false;
            
            sPlayer.transform.SetParent(hand.transform);
            isHolding = true;
            hand.GetComponent<BPlayerHands>().enableHoldSprite();
            rbScript.Grabbed();
            rb.transform.position = hand.transform.position;
            scrpt.ResetTarget();
            grabbedObject = sPlayer;
        } else
        {
            ResetActions();
            Debug.Log("GRABBING FAILED");
        }

        grabRoutine = null;
        isGrabbing = false;
        yield return null;
    }

    IEnumerator Dropping()
    {
        //GameObject hand = grabbedObject.transform.parent.gameObject;
        isDropping = true;
        var handScript = grabHand.GetComponent<MoveToTarget>();
        var scrpt = grabbedObject.GetComponent<SPlayerControls>();

        handScript.SetTarget(dropLocation);

        while(grabHand != null && Vector3.Distance(grabHand.transform.position, dropLocation.position) >= .3)
        {
            yield return fixedWait;
        }
        
        //playerBody.angularDrag = 0.05f;
        
        scrpt.Dropped(true,true);

        ResetActions();
        //isHolding = false;
        //handScript.ResetTarget();
        //handScript.ResetSpeed();
        //grabbedObject = null;
        //grabHand = null;
        // escapeTimer = 0;
        //isDropping = false;
        dropRoutine = null;
        yield return null;
    }

    IEnumerator Throw(InputAction.CallbackContext ctx)
    {
        isThrowing = true; 
       
        var hand = grabbedObject.transform.parent.gameObject;
        var handScript = hand.GetComponent<MoveToTarget>();
        if (handScript.defaultTarget != handScript.targetTransform)
        {
            isThrowing = false;
            yield return null;
        }
        var sPlayerScript = grabbedObject.GetComponent<SPlayerControls>();
        handScript.pauseReposition = true;
        Vector3 relativePos = Vector3.zero;
        Vector3 changePos = Vector3.zero;
        Vector3 origHandPos = handScript.defaultTarget.transform.position;
        while (isThrowing && isHolding && !fire)
        {
            relativePos = crosshair.transform.position - hand.transform.position;
            changePos = (relativePos.magnitude / (relativePos.magnitude + 1f)) * relativePos.normalized * 0.75f;
            changePos *= -1;
            hand.transform.position = origHandPos - changePos;
          
            yield return fixedWait;
        }

        if (fire && !isDropping)
        {
            sPlayerScript.playerBody.AddForce(changePos * 60, ForceMode.Impulse);
            sPlayerScript.ThrowAnim();

            ResetActions();
            //playerBody.angularDrag = 0.05f;
            //isHolding = false;
            //grabbedObject = null;
            //grabHand = null;
            //escapeTimer = 0;
        }

        handScript.pauseReposition = false;
        isThrowing = false;
        throwRoutine = null;
        fire = false;
        yield return null;
    }

    public void Kill()
    {
        deathParticles.Play();
        foreach(SpriteRenderer rend in renderers)
        {
            rend.enabled = false;
        }
        playerBody.GetComponent<Collider>().enabled = false;
        ResetActions();
        StartCoroutine(BSpawnDelay(startingPos));
    }

    IEnumerator BSpawnDelay(Transform spawnLocation)
    {
        yield return new WaitForSeconds(1);
        playerBody.transform.position = spawnLocation.position;
        playerBody.GetComponent<Collider>().enabled = true;
        foreach (SpriteRenderer rend in renderers)
        {
            rend.enabled = true;
        }
    }
}
