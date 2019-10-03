using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;

// This script verifies the hand position.
public class Verify : MonoBehaviour
{
    public bool positionsCorrect;
    public GameObject hand;
    public GameObject headset;
    private ControllerCheck controller;
    private HeadCheck head;
    private float WaitTime;
    
    public GameObject manager, seen, unseen;
    private ViveSR.anipal.Eye.GazeHeadPos gazeHead;

    private void Start()
    {
        gazeHead = manager.GetComponent<ViveSR.anipal.Eye.GazeHeadPos>();

        positionsCorrect = false;
        controller = hand.GetComponent<ControllerCheck>();
        head = headset.GetComponent<HeadCheck>();
    }

    private void Update()
    {

        while (controller.handPosition == true && gazeHead.angularError < 2)
        {
            WaitTime = WaitTime + Time.deltaTime;
        }

        if ((gazeHead.angularError < 2 && controller.handPosition == true) ||
            (gazeHead.angularError > 2 && controller.handPosition == false) ||
            (gazeHead.angularError < 2 && controller.handPosition == false))
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

        if (WaitTime > .5)
        {
            positionsCorrect = true;
            WaitTime = 0;
        }
    }
}