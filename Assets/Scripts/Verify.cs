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

    private void Start()
    {
        positionsCorrect = false;
        controller = hand.GetComponent<ControllerCheck>();
        head = headset.GetComponent<HeadCheck>();
    }

    private void Update()
    {
        if (head.headPosition == true && controller.handPosition == true)
        {
            WaitTime++;
        }

        if ((head.headPosition == false && controller.handPosition == true) ||
            (head.headPosition == true && controller.handPosition == false) ||
            (head.headPosition == false && controller.handPosition == false))
        {
            positionsCorrect = false;
        }
        
        if (positionsCorrect == true)
        {
            Debug.Log("Positions are correct");
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