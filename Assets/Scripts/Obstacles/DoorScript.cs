using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject killBox;
    private bool fullyOpen;
    private bool fullyClosed;
    private bool close;
    private bool open;
    private Coroutine openRoutine;
    private Coroutine closeRoutine;
    private WaitForFixedUpdate fixedWait;
    private Vector3 startingPos;
    private Vector3 openPos;
    public float openAmnt = 6f;
    public float closeSpeed = .5f;
    public float openSpeed = .25f;
    private void Awake()
    {
        fixedWait = new WaitForFixedUpdate();
        startingPos = transform.position;
        openPos = new Vector3(startingPos.x,startingPos.y + openAmnt, startingPos.z);
    }
    public void OpenDoor()
    {
        killBox.SetActive(false);
        if (closeRoutine != null)
        {
            StopCoroutine(closeRoutine);
        }
        
        openRoutine = StartCoroutine(Open());
    }

    public void CloseDoor()
    {
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
        }
        killBox.SetActive(true);
        
        closeRoutine = StartCoroutine(Close());
    }

    IEnumerator Open()
    {
        open = false;
        close = false;
        fullyClosed = false;
        while (!fullyOpen && !close)
        {
            transform.position = Vector3.Lerp(transform.position, openPos, closeSpeed);
            if (Vector3.Distance(transform.position, openPos) <= .1)
            {
                fullyOpen = true;
            }
                yield return fixedWait;
        }

        yield return null;
    }

    IEnumerator Close()
    {
        close = true;
        open = false;
        fullyOpen = false;
        while (!fullyClosed && !open)
        {
            transform.position = Vector3.Lerp(transform.position, startingPos, closeSpeed);
            if (Vector3.Distance(transform.position, startingPos) <= .1)
            {
                fullyClosed = true;
            }
            yield return fixedWait;
        }
    }
}
