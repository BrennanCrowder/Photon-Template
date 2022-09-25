using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesKill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SPlayer")
        {
            other.transform.parent.GetComponent<SPlayerControls>().Kill();
        }
    }
}
