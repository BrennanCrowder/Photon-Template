using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCrushScript : MonoBehaviour
{
    public Transform SLocation;
    public bool enableCrush;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SPlayer" && enableCrush)
        {
            Debug.Log("Crushing Player...");
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.transform.parent.GetComponent<SPlayerControls>().Kill();
        }
    }
}
