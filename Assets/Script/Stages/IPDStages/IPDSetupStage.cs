using System;
using System.IO;
using UnityEngine;

public class IPDSetupStage : Stage
{
    /// <summary>
    /// Stores the distance away from the user the dartboard should be
    /// </summary>
    private readonly float _distance;
    /// <summary>
    /// Stores amount to change IPD by every frame that the user is giving input
    /// </summary>
    private readonly float _dx;
    /// <summary>
    /// Stores true if the user is using their left eye and false if they are using their right
    /// </summary>
    private bool _usingLeftEye;

    /// <summary>
    /// Constructor which stores all parameters
    /// </summary>
    /// <param name="usingLeftEye">True if the user is using their left eye and false if they are using their right</param>
    /// <param name="dx">The amount to change the IPD by every frame that the user is giving input</param>
    /// <param name="distance">Optional: the distance away from the user the dartboard should be</param>
    public IPDSetupStage(bool usingLeftEye, float dx, float distance=6) {
        _usingLeftEye = usingLeftEye;
        _distance = distance;
        _dx = dx;
    }
    
    /// <summary>
    /// Activates the dartboard and deactivates everything else
    /// </summary>
    public override void Start() {
        StageStatic.GameObjects["startButton"].SetActive(false);
        StageStatic.GameObjects["instructions"].SetActive(false);
        StageStatic.GameObjects["target"].SetActive(true);
        StageStatic.GameObjects["coverEye"].SetActive(false);
        StageStatic.GameObjects["left"].SetActive(true);
        StageStatic.GameObjects["right"].SetActive(true);
    }

    /// <summary>
    /// Adjusts position/size/distance of dartboard and adjusts IPD based on user input
    /// </summary>
    public override void Update() {
        // StageStatic.moveDartboardTo(_distance, 5, 0, 0);

        // Enable eye data collection here
        if (StageStatic.EyeDataCol.worldPosL != new Vector3(0, 0, 0))
        {
            StageStatic.GameObjects["left"].transform.position = StageStatic.EyeDataCol.camL.transform.position
                + StageStatic.EyeDataCol.camL.transform.TransformDirection(StageStatic.EyeDataCol.L_Direction) * 150;
        }
        if (StageStatic.EyeDataCol.worldPosR != new Vector3(0, 0, 0))
        {
            StageStatic.GameObjects["right"].transform.position = StageStatic.EyeDataCol.camR.transform.position 
                + StageStatic.EyeDataCol.camR.transform.TransformDirection(StageStatic.EyeDataCol.R_Direction) * 150;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.Log("switching eye, using left eye =" + _usingLeftEye);
            StageStatic.GameObjects["camera"].GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
            _usingLeftEye = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("switching eye, using right eye =" + _usingLeftEye);
            StageStatic.GameObjects["camera"].GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;
            _usingLeftEye = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("switching eye, using both eyes");
            StageStatic.GameObjects["camera"].GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Both;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StageStatic.GameObjects["coverEye"].SetActive(!StageStatic.GameObjects["coverEye"].activeSelf);

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("saved " + StageStatic.IPD + " at " + Application.persistentDataPath);
            File.Delete(Application.persistentDataPath + "/" + StageStatic.name + "_" + "IPD.txt");
            StreamWriter sw = File.AppendText(Application.persistentDataPath + "/" + StageStatic.name + "_" + "IPD.txt");
            sw.WriteLine(StageStatic.IPD);
            sw.Close();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            try
            {
                StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + StageStatic.name + "_" + "IPD.txt");
                StageStatic.IPD = float.Parse(sr.ReadLine());
                Debug.Log("loaded " + StageStatic.IPD + " from " + Application.persistentDataPath);
                sr.Close();
            }
            catch (FileNotFoundException e)
            {
                Debug.Log("You have not saved an IPD under this patient.");
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StageStatic.IPD = 0;
            Debug.Log("reset ipd");
        }


        float change = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            change = -_dx;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            change = _dx;
        }

        if (_usingLeftEye)
            change *= -1;
        StageStatic.changeIPDBy(change);
    
    }
    
    /// <summary>
    /// Checks if the user indicated that they are happy with the displayed IPD
    /// </summary>
    /// <returns>True if the user is pressing A and false otherwise</returns>
    public override bool Finished() {
        return Input.GetKeyDown(KeyCode.A);
    }

    /// <summary>
    /// Unused (code inside is commented because was not working)
    /// </summary>
    public override void End() {
        // // User thinks they are looking at the dartboard with their eye now
        //
        // // Get the local direction that the user is facing
        // Vector3 dir = StageStatic.EyeDataCol.L_Direction;
        // // Compare the local direction that the user is looking to the local direction that the dartboard is in
        // var theta = Vector3.Angle(dir, Vector3.forward);
        // theta = Math.Abs(theta) * (float)Math.PI/180; // Based on left/right eye, just modify
        // // Find the final amount based on this and set the final IPD to this
        // var ipd = (float)Math.Tan(theta) * distance;
        // Debug.Log("Calculated IPD: " + ipd);
        // StageStatic.setIPD(ipd);
    }
}
