using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKill : MonoBehaviour
{
    SPlayerControls sScript;
    BPlayerControls bScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SPlayer" || other.tag == "BPlayer")
        {
            if (other.transform.parent.TryGetComponent<BPlayerControls>(out bScript))
            {
                bScript.Kill();
            }
            else
            {
                sScript = other.transform.parent.GetComponent<SPlayerControls>();
                sScript.Kill();
            }
        }
    }
}
