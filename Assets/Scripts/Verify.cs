using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script verifies the hand position.
public class Verify : MonoBehaviour
{
    private GameObject headbar_stable, headbar_variable;
    private bool head_Transform, position_acceptable;
    public bool positionsCorrect;
    public GameObject hand;
    private GameObject headset, rig;
    private ControllerCheck controller;
    private float WaitTime;
    
    public GameObject manager, seen, unseen;
    private ViveSR.anipal.Eye.GazeHeadPos gazeHead;

    private void Start()
    {
        gazeHead = manager.GetComponent<ViveSR.anipal.Eye.GazeHeadPos>();
        head_Transform = false;
        positionsCorrect = false;
        controller = hand.GetComponent<ControllerCheck>();
        headbar_stable = GameObject.FindGameObjectWithTag("headPosition");
        headbar_variable = GameObject.FindGameObjectWithTag("variable");
        headset = GameObject.FindGameObjectWithTag("Camera");
        rig = GameObject.FindGameObjectWithTag("CameraRig");
    }

    private void Update()
    {
        if (headbar_stable.transform.position.x - .02 < headbar_variable.transform.position.x
            && headbar_stable.transform.position.x + .02 > headbar_variable.transform.position.x
            && headbar_stable.transform.position.y - .02 < headbar_variable.transform.position.y
            && headbar_stable.transform.position.y + .02 > headbar_variable.transform.position.y
            && headbar_stable.transform.position.z - .02 < headbar_variable.transform.position.z
            && headbar_stable.transform.position.z + .02 > headbar_variable.transform.position.z)
        {
            position_acceptable = true;
        }
        else
        {
            position_acceptable = false;   
        }

        if (headbar_stable.transform.parent == headset.transform)
        {
            head_Transform = false;
        }

        if ((headbar_stable.transform.parent == rig.transform) && position_acceptable == true)
        {
            head_Transform = true;
        }

        if ((headbar_stable.transform.parent == rig.transform) && position_acceptable == false)
        {
            head_Transform = false;
        }

        if (controller.handPosition == true && gazeHead.ready == true && head_Transform == true)
        {
            Debug.Log("positions correct");
            WaitTime+=Time.deltaTime;
        }
        else
        {
            positionsCorrect = false;
        }
        
        if (positionsCorrect == true)
        {
            Debug.Log("Ready to Start");
        }

        if (hand.activeSelf == false)
        {
            controller.handPosition = false;
        }

        if (WaitTime > 1)
        {
            positionsCorrect = true;
            WaitTime = 0;
        }
    }
}