using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SPlayer")
        {
            Debug.Log("Dropped!");
            other.transform.parent.GetComponent<SPlayerControls>().Dropped();
        }
    }
}
