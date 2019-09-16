using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script accesses the position of the controller being used in the trials. 
// If either controller is in the correct position, a message will print that 
// states the hand position is correct and the bool "handPosition" will be set to true.
// If the controller leaves the desired hand position, the bool will be set to false
// and a message will print letting the experimenter know that the hand position is no longer correct.

public class ControllerCheck : MonoBehaviour
{
    public bool handPosition;

    // Start is called before the first frame update
    void Start()
    {
        handPosition = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("leftController") || other.gameObject.CompareTag("rightController"))
        {
            handPosition = true;
            Debug.Log("Hand Position correct");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("leftController") || other.gameObject.CompareTag("rightController"))
        {
            handPosition = false;
            Debug.Log("Hand Position lost");
        }
    }

}
