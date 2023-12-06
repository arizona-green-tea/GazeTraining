using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public static class StageStatic {
    /// <summary>
    /// Stores all of the game objects used in the experiment
    /// </summary>
    public static Dictionary<string, GameObject> GameObjects;
    
    /// <summary>
    /// Stores all of the audio clips used in the experiment
    /// </summary>
    public static Dictionary<string, AudioSource> Audios;
    
    /// <summary>
    /// Stores the EyeDataCol object which gets eye data information
    /// </summary>
    public static EyeDataCol EyeDataCol;
    
    /// <summary>
    /// Store the starting position/rotation of the camera
    /// </summary>
    public static Vector3 StartingCameraPosition, StartingCameraRotation;
    
    /// <summary>
    /// True if there is currently a user with the VR headset and false otherwise
    /// </summary>
    public static bool HasActiveUser = false;
    
    /// <summary>
    /// Stores if the dartboard will be positioned relative to the world (val: true) or user (val: false)
    /// </summary>
    public static bool RelativeToWorld = false;
    
    /// <summary>
    /// Stores the user's IPD value
    /// </summary>
    public static float leftIPD = 0.03f;
    public static float rightIPD = 0;

    /// <summary>
    /// Sets all of the necessary information about the experiment setup
    /// </summary>
    /// <param name="allGameObjects">The game objects used in the experiment</param>
    /// <param name="allAudio">The audio clips used in the experiment</param>
    /// <param name="eyeDataCollection">The EyeDataCol object to get eye data from</param>
    public static void setInformation(Dictionary<string, GameObject> allGameObjects, Dictionary<string, AudioSource> allAudio,
    EyeDataCol eyeDataCollection) {
        GameObjects = allGameObjects;
        Audios = allAudio;
        EyeDataCol = eyeDataCollection;
    }

    /// <summary>
    /// Stops all currently playing audio
    /// </summary>
    public static void stopAllAudio() {
        foreach (KeyValuePair<string, AudioSource> audio in Audios) {
            audio.Value.Stop();
        }
    }

    /// <summary>
    /// Moves the start button to a certain distance away from the user
    /// </summary>
    /// <param name="distance">The distance away from the user to move the start button to</param>
    public static void moveStartButtonTo(double distance) {
        moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
            GameObjects["startButton"], new Vector3(0, 0, (float)-distance));
    }

    /// <summary>
    /// Moves the instructions to a certain distance away from the user
    /// </summary>
    /// <param name="distance">The distance away from the user to move the instructions to</param>
    public static void moveInstructionsTo(double distance) {
        moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
            GameObjects["instructions"], new Vector3(0, 0, (float)-distance));
    }
    
    /// <summary>
    /// Handles the movement of the dartboard
    /// </summary>
    /// <param name="distance">the distance of the dartboard from the user</param>
    /// <param name="visualAngle">the size of the dartboard, in terms of visual angle (degrees)</param>
    /// <param name="xAngle">the horizontal position of the dartboard, in visual angle (degrees)</param>
    /// <param name="yAngle">the vertical position of the dartboard, in visual angle (degrees)</param>
    public static void moveDartboardTo(double distance, double visualAngle, double xAngle, double yAngle) {
        if (!RelativeToWorld)
        {
           moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
               GameObjects["target"], new Vector3(0, 0, (float)-distance), visualAngle, -xAngle, yAngle);
        }
        else
        {
           moveToView(StartingCameraPosition, StartingCameraRotation,
               GameObjects["target"], new Vector3(0, 0, (float)-distance), visualAngle, -xAngle, yAngle);
        }
    }

    public static void moveSetUpEnvironment(double distance){
        moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
            GameObjects["setupenvironment"], new Vector3(0, 0, (float)-distance));
    }

    /// <summary>
    /// Used for start button / instructions - moves them right in front of the user, at the specified distance
    /// </summary>
    /// <param name="cameraPos">The current camera position</param>
    /// <param name="cameraAng">The current camera angle</param>
    /// <param name="obj">The object to move into view</param>
    /// <param name="distance">The distance to move the object to away from the camera</param>
    private static void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance) {
        obj.transform.eulerAngles = cameraAng;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y, cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;
    }

    /// <summary>
    /// Moves dartboard to the specified location relative to the user
    /// </summary>
    /// <param name="cameraPos">Current camera position</param>
    /// <param name="cameraAng">Current camera angle</param>
    /// <param name="obj">The object to move (the dartboard)</param>
    /// <param name="distance">The distance to move the dartboard away from the user</param>
    /// <param name="visualAngle">The size to make the dartboard (in visual degrees)</param>
    /// <param name="xAng">The angle to move the dartboard to on the x axis (degrees)</param>
    /// <param name="yAng">The angle to move the dartboard to on the y axis (degrees)</param>
    private static void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance, double visualAngle, double xAng, double yAng) {
        if (xAng == 0 && yAng == 0)
        {
            moveToView(cameraPos, cameraAng, obj, distance);
        }

        obj.transform.LookAt(cameraPos);
        distance += new Vector3((float)(Math.Abs(distance.z) * Math.Tan(xAng * Math.PI / 180)), (float)(Math.Abs(distance.z) * Math.Tan(yAng * Math.PI / 180)), 0);
        distance *= Math.Abs(distance.z) / distance.magnitude;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y, -cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;

        var origSize = 54.8f;
        var neededSize = (float)Math.Tan(visualAngle * Math.PI / 180 * 1 / 2) * distance.magnitude * 2;
        obj.transform.localScale = new Vector3(neededSize / origSize, neededSize / origSize, 1);
    }
    
    /// <summary>
    /// Used to move the left IPD by a certain amount (by the IPD stage)
    /// </summary>
    /// <param name="dx">The amount to change the IPD by</param>
    public static void changeLeftIPDBy(float dx) {
        leftIPD += dx;
        leftIPD = Math.Max(0, leftIPD);
    }

    /// <summary>
    /// Used to move the right IPD by a certain amount (by the IPD stage)
    /// </summary>
    /// <param name="dx">The amount to change the IPD by</param>
    public static void changeRightIPDBy(float dx)
    {
        rightIPD += dx;
        rightIPD = Math.Max(0, rightIPD);
    }


}