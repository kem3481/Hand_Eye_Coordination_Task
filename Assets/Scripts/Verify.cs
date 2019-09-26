using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

// This script verifies the hand position.
public class Verify : MonoBehaviour
{
    public bool positionsCorrect;
    public GameObject hand;
    public GameObject headset;
    private ControllerCheck controller;
    private HeadCheck head;
    private int WaitTime;

    public GameObject manager;
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
        if (controller.handPosition == true && gazeHead.angularError > 2)
        {
            WaitTime++;
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

        if (WaitTime > 100)
        {
            positionsCorrect = true;
            WaitTime = 0;
        }
    }
}