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

    public float controllerAngular, targetAngular;
    public Vector3 controllerPosition, targetPosition, ControllerPosition, TargetPosition;

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
            "Frame Number\t" + "Target Position Angular Difference\t" + "Hand Position Angular Difference\t" + "Gaze Position Angular Difference\t" + Environment.NewLine
                        );


        writeString = stringBuilder.ToString();
        writebytes = Encoding.ASCII.GetBytes(writeString);
        trialStreams.Write(writebytes, 0, writebytes.Length);

    }

    void WriteFile()
    {
        TargetPosition.x = controls.target_x;
        TargetPosition.y = controls.target_y;
        TargetPosition.z = controls.target_z;
        targetPosition.x = TargetPosition.x - headPosition.transform.position.x;
        targetPosition.y = TargetPosition.y - headPosition.transform.position.y;
        targetPosition.z = TargetPosition.z - headPosition.transform.position.z;
        targetAngular = Mathf.Acos((Vector3.Dot(targetPosition, gazeHead.fixationpointPos)) / (Vector3.Magnitude(targetPosition) * Vector3.Magnitude(gazeHead.fixationpointPos))) * Mathf.Rad2Deg;

        ControllerPosition.x = controls.trigger_x;
        ControllerPosition.y = controls.trigger_y;
        ControllerPosition.z = controls.trigger_z;
        controllerPosition.x = ControllerPosition.x - headPosition.transform.position.x;
        controllerPosition.y = ControllerPosition.y - headPosition.transform.position.y;
        controllerPosition.z = ControllerPosition.z - headPosition.transform.position.z;
        controllerAngular = Mathf.Acos((Vector3.Dot(controllerPosition, gazeHead.fixationpointPos)) / (Vector3.Magnitude(controllerPosition) * Vector3.Magnitude(gazeHead.fixationpointPos))) * Mathf.Rad2Deg;

        stringBuilder.Length = 0;
        stringBuilder.Append(
                    Time.frameCount.ToString() + "\t\t" // add frame number object
                    + targetAngular.ToString() + "\t"
                    + controllerAngular.ToString() + "\t"
                    + gazeHead.angularError.ToString() + "\t" +
                    Environment.NewLine
                );
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
