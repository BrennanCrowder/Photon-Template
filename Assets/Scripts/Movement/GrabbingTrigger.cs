using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SPlayer")
        {
            var script = other.transform.GetComponent<SPlayerControls>();
            if (!script.isThrown && !script.isGrabbed)
            {
                Debug.Log("Dropped!");
                script.isThrown = true;
                //script.gameObject.layer = LayerMask.NameToLayer("SPlayer");
                other.transform.GetComponent<SPlayerControls>().Dropped(false,true);
            }
            
        }
    }
}
