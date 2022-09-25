using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SPlayerControls : MonoBehaviour
{
    public float speed = 1f;
    public float jumpPower = 3;
    public float jumpCheckDist = 1f;
    public Rigidbody playerBody;
    public Collider playerCollider;
    public ParticleSystem deathParticles;
    private bool moving;
    private Coroutine moveRountine;
    public SpriteRenderer renderer;
    private Animator animator;
    public bool isGrabbed = false;
    public bool isThrown;
    [Header("Animator Variables")]
    public float minWalkSpeed = 0.1f;
    public Transform startingPos;
    private bool doNotKill;
    private void Awake()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        startingPos.position = transform.position;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moving = false;
            StopCoroutine(moveRountine);
        }
        if (ctx.performed && !isGrabbed)
        {
            moving = true;
            moveRountine = StartCoroutine(SMoving(ctx));
        }

    }
    private int count = 1;
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && CheckGrounded() && !isGrabbed)
        {
            playerBody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        }
        if (ctx.performed && isGrabbed)
        {
            count = transform.root.GetComponent<BPlayerControls>().Escape(count);

        } else if (!isGrabbed && count != 1)
        {
            count = 1;
        }
    }

    public bool CheckGrounded()
    {
        if (Physics.Raycast(playerBody.transform.position, Vector3.down, jumpCheckDist, LayerMask.GetMask("Platform", "BPlayer"), QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return false;
    }

    IEnumerator SMoving(InputAction.CallbackContext ctx)
    {
        while (moving && !isGrabbed)
        {
            playerBody.AddForce(new Vector3(speed * ctx.ReadValue<Vector2>().x, 0, 0), ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
    public void Kill()
    {
        if (doNotKill)
        {
            return;
        }
        // Particle
        // Sound
        // Life?
        // Reset Pos
        deathParticles.Play();
        Dropped(true,true);
        renderer.enabled = false;
        playerBody.GetComponent<Collider>().enabled = false;
        StartCoroutine(SpawnDelay(startingPos));
    }
    public void Kill(Transform spawnLocation)
    {
        if (doNotKill)
        {
            return;
        }
        // Particle
        // Sound
        // Life?
        // Reset Pos
        deathParticles.Play();
        Dropped();
        renderer.enabled = false;
        playerBody.GetComponent<Collider>().enabled = false;
        StartCoroutine(SpawnDelay(spawnLocation));
    }

    public void Grabbed()
    {
        doNotKill = true;
        //playerCollider.isTrigger = true;
        playerBody.gameObject.layer = LayerMask.NameToLayer("Grabbed");
        
        isThrown = false;
        isGrabbed = true;
        playerBody.velocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
        playerBody.useGravity = false;
        playerBody.ResetCenterOfMass();
        playerBody.Sleep();
    }

    public void Dropped(bool unParent = false, bool resetLayer = false)
    {
        doNotKill = false;
        if (unParent)
        {
            transform.SetParent(null);
            
        }
        if (resetLayer)
        {
            playerBody.gameObject.layer = LayerMask.NameToLayer("SPlayer");
        }
        playerBody.useGravity = true;

        isGrabbed = false;
    }

    IEnumerator SpawnDelay(Transform spawnLocation)
    {
        yield return new WaitForSeconds(1);
        playerBody.transform.position = spawnLocation.position;
        playerBody.GetComponent<Collider>().enabled = true;
        renderer.enabled = true;
    }

    // Update Animator
    private void Update()
    {
        animator.SetBool("Walking", playerBody.velocity.magnitude > minWalkSpeed);
        animator.SetBool("Grounded", (CheckGrounded() && !isGrabbed));
        animator.SetBool("Grabbed", isGrabbed);
    }

    public void ThrowAnim()
    {
        Dropped(true,false);
        animator.SetTrigger("Thrown");
    }
}
