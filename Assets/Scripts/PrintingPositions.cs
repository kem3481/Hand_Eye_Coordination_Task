using UnityEngine;
using System;
using System.IO;
using System.Text;

public class PrintingPositions : MonoBehaviour
{
    public string FolderName = "C:\\Users\\kem3481\\Tracking_Data";
    public string FileName = "Test";
    private string OutputDir;

    private ViveSR.anipal.Eye.GazeHeadPos gazeHead; // angular error
    private ControlLevel_Trials controls;
    public GameObject fixationPosition;
    public GameObject headPosition;
    public GameObject manager;
    public GameObject target, penalty;
    public GameObject controller;

    public float controllerAngular, targetAngular, penaltyAngular;
    public Vector3 ControllerPosition, TargetPosition, PenaltyPosition, GazePosition, FixPosition, vector;

    //Gives user control over when to start and stop recording, trigger this with spacebar;
    public bool startWriting;

    //Initialize some containers
    FileStream streams;
    FileStream trialStreams;
    StringBuilder stringBuilder = new StringBuilder();
    String writeString;
    Byte[] writebytes;

    // Start is called before the first frame update
    void Start()
    {
     // angular error between fixation and controller
        gazeHead = manager.GetComponent<ViveSR.anipal.Eye.GazeHeadPos>();
        controls = manager.GetComponent<ControlLevel_Trials>();

        // create a folder 
        string OutputDir = Path.Combine(FolderName, string.Concat(DateTime.Now.ToString("MM-dd-yyyy"), FileName));
        Directory.CreateDirectory(OutputDir);
        
        // create a file to record data
        String trialOutput = Path.Combine(OutputDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "_Results.txt");
        trialStreams = new FileStream(trialOutput, FileMode.Create, FileAccess.Write);
        
        //Call the function below to write the column names
        WriteHeader();
        controller = controls.gamecontroller;

        vector = new Vector3(0, 0, 0);
    }

    void WriteHeader()
    {
        //output file-- order of column names here should match the order you use when writing out each value 
        stringBuilder.Length = 0;
        //add header info
        stringBuilder.Append(
        DateTime.Now.ToString() + "\t" +
        "The file contains hand and eye position for each frame" + Environment.NewLine +
        "The coordinate system is in Unity world coordinates." + Environment.NewLine
        );
        stringBuilder.Append("-------------------------------------------------" +
            Environment.NewLine
            );
        //add column names
        stringBuilder.Append(
            "Frame Number\t" + "Target Position Angular Difference\t" + "Penalty Position Angular Difference\t" + "Hand Position Angular Difference\t" + "Gaze Position Angular Difference\t" + Environment.NewLine
                        );


        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);

    }

    void WriteFile()
    {/*
        GazePosition = gazeHead.gazeDirection.normalized;
        FixPosition = gazeHead.fixationpointPos.normalized;

        target.transform.position = new Vector3(controls.target_x, controls.target_y, controls.target_z);
        target.transform.SetParent(gazeHead.head.transform);
        TargetPosition = target.transform.localPosition;
        TargetPosition = TargetPosition.normalized;
        targetAngular = Mathf.Acos((Vector3.Dot(TargetPosition, FixPosition)) / (Vector3.Magnitude(TargetPosition) * Vector3.Magnitude(FixPosition))) * Mathf.Rad2Deg;

        penalty.transform.position = new Vector3(controls.penalty_x, controls.penalty_y, controls.penalty_z);
        penalty.transform.SetParent(gazeHead.head.transform);
        PenaltyPosition = penalty.transform.localPosition;
        PenaltyPosition = PenaltyPosition.normalized;
        penaltyAngular = Mathf.Acos((Vector3.Dot(PenaltyPosition, FixPosition)) / (Vector3.Magnitude(PenaltyPosition) * Vector3.Magnitude(FixPosition))) * Mathf.Rad2Deg;

        controller.transform.SetParent(gazeHead.head.transform);
        ControllerPosition = controller.transform.localPosition;
        ControllerPosition = ControllerPosition.normalized;
        controllerAngular = Mathf.Acos((Vector3.Dot(ControllerPosition, FixPosition)) / (Vector3.Magnitude(ControllerPosition) * Vector3.Magnitude(FixPosition))) * Mathf.Rad2Deg;
        */
        ControllerPosition = (controller.transform.position - gazeHead.head.transform.position);

        if (controls.targetonObject != null)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(
                        Time.frameCount.ToString() + "\t\t" 
                        + controls.targetonObject.transform.position.ToString("F4") + "\t"
                        + controls.penalty.transform.position.ToString("F4") + "\t"
                        + ControllerPosition.ToString("F4") + "\t"
                        + gazeHead.gazeDirection.ToString("F4") + "\t" +
                        Environment.NewLine
                    );
        }
        if (controls.targetonObject == null)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(
                        Time.frameCount.ToString() + "\t\t" 
                        + vector.ToString("F4") + "\t"
                        + vector.ToString("F4") + "\t"
                        + ControllerPosition.ToString("F4") + "\t"
                        + gazeHead.gazeDirection.ToString("F4") + "\t" +
                        Environment.NewLine
                    );
        }
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);
    }

    public void Update()
    {
      WriteFile();

    }

    public void OnApplicationQuit()
    {
        trialStreams.Close();
    }
}
