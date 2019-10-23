using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USE_States;
using Valve.VR;
using System;
using System.IO;
using System.Text;

// This is the main script of the game. It can acces all variables and real world objects. 
// It is designed using states of which there are 7. 

// The first state titled 'begin' sets the float 'percentCorrect' equal to 0. 
// This float tracks in how many trials the participant hit the target region. 
// The begin state sets gameobject ‘begintext’ false unless it is the first trial. 
// If it is the first trial, this text will be displayed and will explain to the 
// participant how the game will work. It will set the gameobject ‘endtext’ to false. 
// It sets the gameobjects ‘controllerposition’ and ‘handposition’ to true.
// These are the cylinders in which the headset and the game controller should begin the test. 
// It sets the text ‘scoredisplay’ to an empty string. It sets the gameobjects ‘scoretext’ 
// and ‘trigger’ to false. It sets the bool ‘data’ to false. It defines the startTime to
// be the current time when the state is run. On the first trial, it sets the score to 0.
//  It sets the controller position, target position, and penalty positions that are used 
// for reading position at the time of collision equal to 0 to reset them from the previous trial.
//  It also sets the game controller that is not being used to false so that it is not present 
// in the scene. The script will continue on if the ‘positionscorrect’ object from the
// verifypositions script is true.

// The stimOn state sets ‘begintest’ and ‘controllerpostion’ to false. When the state starts, 
// 1 is added to the ‘trials’ int to keep track of trial number. The gameobject ‘orientation’ 
// is randomly set at either 1 or 2.The polarangle is calculated randomly from 0 to 359 degrees 
// and converted to radians. The radius is set to be .5f. The type of target (overlap) and the 
// polar angle (eccentricity) are randomly selected. The target is generated and the position of 
// the target and penalty regions are saved to variables.

// In the collectResponse state, if the target is hit or if the user moves their controller 
// past the target, the position of the controller is written to a variable and the target 
// is destroyed. If too much time has passed between when the target appeared and when the 
// response is collectd, a time out penalty is incurred and the next state begins. If the time 
// out penalty is triggered the next state is a penaltystate which subtracts 500 points from the 
// user’s score. Otherwise, script moves on to a scoring state that will give the user points or 
// remove points based on whether or not they hit the target region. After the score is set for 
// this round, the script moves on to destination.

// In the destination state, if the trial number that was set in the stimOn state is under 360, 
// the script loops back to begin. If the trial number is under 360, the script moves on to feedback.

// In the feedback state, an endtext appears and the overall percent correct is calculated. 




public class ControlLevel_Trials : ControlLevel
{
    // Canvases and Text objects
    public GameObject beginText; // instructions canvas
    public GameObject endText; // thank you for playing canvas
    public Text scoreDisplay;
    public GameObject scoreText;
    
    // Other script declarations
    private Verify verifyPositions;
    private Controls controls; 
    private ControllerCheck controller; 
    private TriggerPull triggered;
    
    // Data Writeout objects
    public string startTime;
    public string endTime;
    public int trials = 0; 
    private int score; 
    [System.NonSerialized]
    public float angle, eccentricity, a;
    [System.NonSerialized]
    public float radius, trigger_x, trigger_y, trigger_z, target_x, target_y, target_z, penalty_x, penalty_y, penalty_z;
    
    // Placeholders for loops and accessing members lists
    private int j, i, k = 0;
    private int orientation;
    private int random1, random2; 
    public int cases;
    
    // Intermediate objects
    private float Timer;
    private int trialScore;
    private int end = 0;
    public bool data;
    public int numberoftrials = 20;
    
    // Physical objects
    private GameObject rightController;
    private GameObject leftController;
    private GameObject headset;
    private GameObject headbar_stable, headbar_variable;
    private GameObject rig;
    
    // Empty Game objects
    private GameObject trigger;
    public GameObject manager; // control levels game object, holding all scirpts

    // Unity Space objects
    public GameObject controllerPosition; // Hand position column
    public GameObject playerPosition; // headset poisiton column
    public GameObject gamecontroller; // Set to which controller is being used (icon)
    public GameObject test; // leftcontroller or right controller in heirarchy
    public GameObject target, fixationpoint;
    public GameObject penalty;
    public GameObject targetonObject;
    public GameObject startingPositions;
    private Vector3 targetDirection;
    private Transform headsetTrans;

    public int hit;
    public int Fix;
    public float percentCorrect;
    private GameObject toggle;
    [System.NonSerialized]
    public GameObject testobject;
    
    // List of 20 of each trial type
    public int[] trialTypes;
    
    public override void DefineControlLevel()
    {
        // Defining States
        State nothing = new State("nothing");
        State headStabilization = new State("Stabilize"); // ensures that the head is in the same position at the beginning of all trials
        State begin = new State("Begin"); // Step 1 and 2 in Procedure
        State stimOn = new State("Stimulus"); // Step 3
        State collectResponse = new State("CollectResponse"); // Step 4
        State penaltyState = new State("TimeOutPenalty");
        State scoreState = new State("Score");
        State destination = new State("Destination"); // sends the script either back to begin or to feeback
        State feedback = new State("Feedback"); // Step 5
        AddActiveStates(new List<State> { nothing, headStabilization, begin, stimOn, collectResponse, penaltyState, scoreState, destination, feedback });
        
        // Accessing other scripts
        verifyPositions = manager.GetComponent<Verify>();
        controls = manager.GetComponent<Controls>();
        controller = controllerPosition.GetComponent<ControllerCheck>();
        triggered = gamecontroller.GetComponent<TriggerPull>();
        rightController = GameObject.FindGameObjectWithTag("rightController");
        leftController = GameObject.FindGameObjectWithTag("leftController");
        headset = GameObject.FindGameObjectWithTag("Camera");
        headbar_stable = GameObject.FindGameObjectWithTag("headPosition");
        headbar_variable = GameObject.FindGameObjectWithTag("variable");
        rig = GameObject.FindGameObjectWithTag("CameraRig");
        trigger = GameObject.FindGameObjectWithTag("Trigger");
        toggle = GameObject.FindGameObjectWithTag("toggle");
        trialTypes = controls.trialTypes;
        scoreDisplay.text = "Score: " + score;

        nothing.AddStateInitializationMethod(() =>
        {
            hit = -1;
            beginText.SetActive(false);
            fixationpoint.SetActive(false);
            endText.SetActive(false);
            controllerPosition.SetActive(false);
            playerPosition.SetActive(true);
            scoreDisplay.text = string.Empty;
            scoreText.SetActive(false);
            trigger.SetActive(false);
            data = false;
        });
        nothing.SpecifyStateTermination(()=> toggle.activeSelf == false, headStabilization);

        headStabilization.AddStateInitializationMethod(() =>
        {
            headbar_stable.transform.SetParent(rig.transform);
        });
        headStabilization.SpecifyStateTermination(() => toggle.activeSelf == true, begin);

        begin.AddStateInitializationMethod(() =>
        {
            hit = -1;
            headbar_stable.SetActive(true);
            headbar_variable.SetActive(true);
            UnityEngine.Random.InitState((int)DateTime.Now.Second);
            percentCorrect = 0;
            beginText.SetActive(false);
            endText.SetActive(false);
            controllerPosition.SetActive(true);
            playerPosition.SetActive(true);
            scoreDisplay.text = string.Empty;
            scoreText.SetActive(false);
            trigger.SetActive(false);
            fixationpoint.SetActive(true);
            data = false;
            startTime = System.DateTime.UtcNow.ToString("HH:mm:ss");
            if (trials == 0)
            {
                score = 0;
            }

            trialScore = 0;
            if (trials < 1)
            {
                beginText.SetActive(true);
            }
            endText.SetActive(false);

            trigger_x = 0;
            trigger_y = 0;
            trigger_z = 0;
            target_x = 0;
            target_y = 0;
            target_z = 0;
            penalty_x = 0;
            penalty_y = 0;
            penalty_z = 0;
            
            if (gamecontroller == rightController)
            {
                leftController.SetActive(false);
            }
            if (gamecontroller == leftController)
            {
                rightController.SetActive(false);
            }

        });
        begin.SpecifyStateTermination(() => verifyPositions.positionsCorrect, stimOn);
        begin.AddDefaultTerminationMethod(() => beginText.SetActive(false));

        stimOn.AddStateInitializationMethod(() =>
        {
        headbar_stable.SetActive(false);
        headbar_variable.SetActive(false);
        fixationpoint.SetActive(false);
        beginText.SetActive(false);
        controllerPosition.SetActive(false);
        trials++;

        orientation = UnityEngine.Random.Range(0, 2);
        angle = (Mathf.Deg2Rad * UnityEngine.Random.Range(0, 359));
        radius = 1.5f;
        
        random2 = UnityEngine.Random.Range(0, controls.trialTypes.Length);
        random1 = controls.trialTypes[random2];
        for (int i = 0; i < 9; i++)
        {
            if (random1 == i)
            {
                eccentricity = controls.allAngles[i] * (Mathf.Deg2Rad);
                target = controls.allTargets[i];
            }
        }
        
        for (int i = random2; i < controls.trialTypes.Length - 1; i++)
        {
            controls.trialTypes[i] = controls.trialTypes[i+1];
        }
        
        Array.Resize(ref controls.trialTypes, controls.trialTypes.Length - 1);

            if (testobject == null)
            {
                testobject = Instantiate(target);
                testobject.transform.parent = playerPosition.transform;

                targetonObject = GameObject.FindGameObjectWithTag("Object");
                penalty = GameObject.FindGameObjectWithTag("PenaltyonTarget");

                targetDirection = new Vector3((radius * Mathf.Sin(eccentricity) * Mathf.Cos(angle)), (radius * Mathf.Sin(eccentricity) * Mathf.Sin(angle)), (radius * Mathf.Cos(eccentricity)));

                testobject.transform.localPosition = new Vector3(targetDirection.x, targetDirection.y, targetDirection.z);
                if (orientation == 1)
                {
                    testobject.transform.eulerAngles = new Vector3(0f, -angle * Mathf.Rad2Deg + 90, 0f);
                }
                if (orientation == 0)
                {
                    testobject.transform.eulerAngles = new Vector3(0f, (-angle * Mathf.Rad2Deg) + 270, 0f);
                }
            }
                target_x = targetonObject.transform.position.x;
                target_y = targetonObject.transform.position.y;
                target_z = targetonObject.transform.position.z;
                penalty_x = penalty.transform.position.x;
                penalty_y = penalty.transform.position.y;
                penalty_z = penalty.transform.position.z;
            
        
        });
        stimOn.AddTimer(.1f, collectResponse);

        collectResponse.AddStateInitializationMethod(() =>
        {
            Timer = 0;
        });
        collectResponse.AddUpdateMethod(() =>
        {

            
            if (triggered.passedRadius == true)
            {
                trigger_x = test.transform.position.x;
                trigger_y = test.transform.position.y;
                trigger_z = test.transform.position.z;
                data = true;
                Destroy(testobject);
            }


            // Colliders not triggers
            if (trigger.activeSelf == true)
            {
                trigger_x = test.transform.position.x;
                trigger_y = test.transform.position.y;
                trigger_z = test.transform.position.z;
                data = true;
                Destroy(testobject);
            }

            Timer += Time.deltaTime;
        });
        collectResponse.SpecifyStateTermination(() => testobject == null, scoreState);
        collectResponse.SpecifyStateTermination(() => Timer > 5f, penaltyState);

        penaltyState.AddStateInitializationMethod(() =>
        {
            Destroy(testobject);
            trialScore = -500;
        });
        penaltyState.AddTimer(.01f, destination);

        scoreState.AddStateInitializationMethod(() =>
        {
            if (triggered.targetTouched == true)
            {
                trialScore = 100;
                triggered.targetTouched = false;
                percentCorrect++;
                score = score + trialScore;
                scoreDisplay.color = Color.green;
                scoreDisplay.text = "+ 100 \n " + "Score: " + score;
                scoreText.SetActive(true);
                hit = 1;
            }
            else
            {
                trialScore = 0;
            }

            if (triggered.penaltyTouched == true)
            {
                trialScore = -100;
                triggered.penaltyTouched = false;
                score = score + trialScore;
                scoreDisplay.color = Color.red;
                scoreDisplay.text = "- 100 \n " + "Score: " + score;
                scoreText.SetActive(true);
                hit = 0;
            }

            trigger.SetActive(false);

        });
        scoreState.AddTimer(1.5f, destination);

        destination.AddStateInitializationMethod(() =>
        {
            scoreText.SetActive(false);

            if (trials < 180)
            {
                end = 1;
            }
            else
            {
                end = 2;
            }
            
        });
        destination.SpecifyStateTermination(() => end == 1, begin);
        destination.SpecifyStateTermination(() => end == 2, feedback);

        feedback.AddStateInitializationMethod(() =>
        {
            
            endText.SetActive(true);
            percentCorrect = percentCorrect / 180;
        });

    }

}
