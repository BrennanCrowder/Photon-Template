using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTriggerScript : MonoBehaviour
{
    private GameObject body;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SPlayer")
        {
            Debug.Log("SPlayer ON Platform");
            body = other.gameObject.transform.root.gameObject; // Get Reference to SPlayer root
            body.transform.SetParent(transform.parent); // Set Parent to Body Platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SPlayer")
        {
            Debug.Log("SPlayer OFF Platform");
            body.transform.SetParent(null);
            body = null;
        }
    }

    public GameObject GetBody()
    {
        return body;
    }

    //public GameObject
}
