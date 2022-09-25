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
    private Renderer renderer;
    private Animator animator;
    public bool isGrabbed = false;

    [Header("Animator Variables")]
    public float minWalkSpeed = 0.1f;    

    private void Awake()
    {
        renderer = playerBody.GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
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

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (CheckGrounded())
        {
            playerBody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
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
        while (moving)
        {
            playerBody.AddForce(new Vector3(speed * ctx.ReadValue<Vector2>().x, 0, 0), ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public void Kill(Transform spawnLocation)
    {
        // Particle
        // Sound
        // Life?
        // Reset Pos
        deathParticles.Play();
        renderer.enabled = false;
        playerBody.GetComponent<Collider>().enabled = false;
        StartCoroutine(SpawnDelay(spawnLocation));
    }

    public void Grabbed()
    {
        playerCollider.enabled = false;
        playerBody.velocity = Vector3.zero;
        playerBody.useGravity = false;
        isGrabbed = true;
    }

    public void Dropped()
    {
        playerCollider.enabled = true;
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
        animator.SetBool("Grounded", CheckGrounded());
        animator.SetBool("Grabbed", isGrabbed);
        // NEED TO ADD THROWN ANIMATION
    }
}
