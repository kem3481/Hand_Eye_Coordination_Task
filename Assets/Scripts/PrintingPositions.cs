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
    public GameObject controller;
    public GameObject fixationPosition;
    public GameObject headPosition;
    public GameObject manager;
    private GameObject target;

    public float controllerAngular, targetAngular, penaltyAngular;
    public Vector3 ControllerPosition, TargetPosition, PenaltyPosition;

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
    {
        if (controls.targetonObject == null)
        {
            target.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            target.transform.position = controls.targetonObject.transform.position;
        }
        target.transform.SetParent(gazeHead.head.transform);
        target.transform.localPosition = Vector3.forward;
        TargetPosition = target.transform.localPosition;
        
        targetAngular = Mathf.Acos((Vector3.Dot(TargetPosition, gazeHead.fixationpointPos)) / (Vector3.Magnitude(TargetPosition) * Vector3.Magnitude(gazeHead.fixationpointPos))) * Mathf.Rad2Deg;

        PenaltyPosition.x = controls.penalty_x;
        PenaltyPosition.y = controls.penalty_y;
        PenaltyPosition.z = controls.penalty_z;
        penaltyAngular = Mathf.Acos((Vector3.Dot(PenaltyPosition, gazeHead.fixationpointPos)) / (Vector3.Magnitude(PenaltyPosition) * Vector3.Magnitude(gazeHead.fixationpointPos))) * Mathf.Rad2Deg;

        ControllerPosition = controls.gamecontroller.transform.position;
        controllerAngular = Mathf.Acos((Vector3.Dot(ControllerPosition, gazeHead.fixationpointPos)) / (Vector3.Magnitude(ControllerPosition) * Vector3.Magnitude(gazeHead.fixationpointPos))) * Mathf.Rad2Deg;

        stringBuilder.Length = 0;
        stringBuilder.Append(
                    Time.frameCount.ToString() + "\t\t" // add frame number object
                    + targetAngular.ToString() + "\t"
                    + penaltyAngular.ToString() + "\t"
                    + controllerAngular.ToString() + "\t"
                    + gazeHead.angularError.ToString() + "\t" +
                    Environment.NewLine
                );
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);

        Debug.DrawRay(Vector3.zero, TargetPosition, Color.red);
        Debug.DrawRay(Vector3.zero, ControllerPosition, Color.green);
        Debug.DrawRay(gazeHead.binocularEIHorigin, gazeHead.gazeDirection, Color.blue);
        Debug.DrawRay(Vector3.zero, gazeHead.fixationpointPos, Color.magenta);
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
