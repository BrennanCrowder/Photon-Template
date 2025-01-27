using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTriggerScript : MonoBehaviour
{
    private GameObject body;
    public bool track;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SPlayer" )
        {
            body = other.gameObject; // Get Reference to SPlayer root
           // Debug.Log("SPlayer ON Platform");
            if (!body.GetComponent<SPlayerControls>().isGrabbed && track)
            {
                
                body.transform.SetParent(transform.parent); // Set Parent to Body Platform
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SPlayer")
        {
            //Debug.Log("SPlayer OFF Platform");
            if (!body.GetComponent<SPlayerControls>().isGrabbed && track)
            {
                body.transform.SetParent(null);
            }
            body = null;
        }
    }

    public GameObject GetBody()
    {
        return body;
    }

    public void ResetBody()
    {
        body = null;
    }
}
