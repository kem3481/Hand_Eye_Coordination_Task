using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script verifies the hand position.
public class Verify : MonoBehaviour
{
    public bool positionsCorrect;
    public GameObject hand;
    public GameObject headset;
    private ControllerCheck controller;
    private float WaitTime;
    
    public GameObject manager, seen, unseen;
    private ViveSR.anipal.Eye.GazeHeadPos gazeHead;

    private void Start()
    {
        gazeHead = manager.GetComponent<ViveSR.anipal.Eye.GazeHeadPos>();

        positionsCorrect = false;
        controller = hand.GetComponent<ControllerCheck>();
    }

    private void Update()
    {
        if (controller.handPosition == true && gazeHead.ready == true)
        {
            Debug.Log("positions correct");
            WaitTime+=Time.deltaTime;
        }

        if ((gazeHead.ready == false && controller.handPosition == true) ||
            (gazeHead.ready == true && controller.handPosition == false) ||
            (gazeHead.ready == false && controller.handPosition == false))
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