using UnityEngine;
using System;
using System.IO;
using System.Text;

public class FullPositions : MonoBehaviour
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
    public float angularError;
    public float controllerAngular, targetAngular, penaltyAngular, gazeAngular;
    public Vector3 ControllerPosition, TargetPosition, PenaltyPosition, GazePosition, FixPosition, vector;
    public float zero = 0;
    public int hit;
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
            "Frame Number\t" + "Target Position_x\t" + "Target Position_y\t" + "Target Position_z\t" + "Penalty Position_x\t" + "Penalty Position_y\t" + "Penalty Position_z\t" + "Hand Position_x\t" + "Hand Position_y\t" + "Hand Position_z\t" + "Gaze Position_x\t" + "Gaze Position_y\t" + "Gaze Position_z\t" + "Hit(1) or Miss(0)\t"+ Environment.NewLine
                        );


        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);

    }

    void WriteFile()
    {
        ControllerPosition = (controller.transform.position);
        TargetPosition = (controls.targetonObject.transform.position);
        PenaltyPosition = (controls.penalty.transform.position);
        GazePosition = (gazeHead.gazeDirection);
        
        hit = controls.score;

        if (controls.targetonObject != null)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(
                        Time.frameCount.ToString() + "\t"
                        + TargetPosition.x.ToString("F4") + "\t"
                        + TargetPosition.y.ToString("F4") + "\t"
                        + TargetPosition.z.ToString("F4") + "\t"
                        + PenaltyPosition.x.ToString("F4") + "\t"
                        + PenaltyPosition.y.ToString("F4") + "\t"
                        + PenaltyPosition.z.ToString("F4") + "\t"
                        + ControllerPosition.x.ToString("F4") + "\t"
                        + ControllerPosition.y.ToString("F4") + "\t"
                        + ControllerPosition.z.ToString("F4") + "\t"
                        + GazePosition.x.ToString("F4") + "\t"
                        + GazePosition.y.ToString("F4") + "\t"
                        + GazePosition.z.ToString("F4") + "\t" 
                        + hit.ToString() + "\t" +
                        Environment.NewLine
                    );
        }
        
        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);
        Debug.DrawRay(Vector3.zero, ControllerPosition, Color.black);
        Debug.DrawRay(Vector3.zero, TargetPosition, Color.green);
        Debug.DrawRay(Vector3.zero, PenaltyPosition, Color.red);
        Debug.DrawRay(Vector3.zero, GazePosition, Color.magenta);
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
