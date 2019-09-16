using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to check that the participant is positioned correctly in the unity scene.
public class HeadCheck : MonoBehaviour
{
    public bool headPosition;

    // Start is called before the first frame update
    void Start()
    {
        headPosition = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Camera"))
        {
            headPosition = true;
            Debug.Log("Head Position Correct");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Camera"))
        {
            headPosition = false;
        }
    }
}