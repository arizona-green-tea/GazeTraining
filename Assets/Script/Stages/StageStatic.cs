using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public static class StageStatic {
    public static Dictionary<string, GameObject> GameObjects;
    public static Dictionary<string, AudioSource> Audios;
    public static EyeDataCol EyeDataCol;
    public static Vector3 startingCameraPosition, startingCameraRotation;
    public static bool hasActiveUser = false;
    // Stores if the dartboard will be positioned relative to the world (val: true) or user (val: false)
    public static bool relativeToWorld = false;
    public static float IPD = 0.03f;

    public static void setInformation(Dictionary<string, GameObject> allGameObjects, Dictionary<string, AudioSource> allAudio,
    ViveSR.anipal.Eye.EyeDataCol eyeDataCollection) {
        GameObjects = allGameObjects;
        Audios = allAudio;
        EyeDataCol = eyeDataCollection;
    }

    // Stops all audio currently playing
    public static void stopAllAudio() {
        foreach (KeyValuePair<string, AudioSource> audio in Audios) {
            audio.Value.Stop();
        }
    }

    // Handles moving the start button to a certain distance away from the user
    public static void moveStartButtonTo(double distance) {
        moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
            GameObjects["startButton"], new Vector3(0, 0, (float)-distance));
    }

    // Handles moving the instructions to a certain distance away from the user
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
        if (!relativeToWorld) {
            moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
                GameObjects["target"], new Vector3(0, 0, (float)-distance), visualAngle, -xAngle, yAngle);
        } else {
            moveToView(startingCameraPosition, startingCameraRotation,
                GameObjects["target"], new Vector3(0, 0, (float)-distance), visualAngle, -xAngle, yAngle);
        }
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
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y + 180, cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;
    }

    // Used for dartboard - moves it to the specified location relative to the user
    private static void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance, double visualAngle, double xAng, double yAng) {
        if (xAng == 0 && yAng == 0) {
            moveToView(cameraPos, cameraAng, obj, distance);
        }
        
        obj.transform.LookAt(cameraPos);

        distance += new Vector3((float)(Math.Abs(distance.z) * Math.Tan(xAng * Math.PI/180)), (float)(Math.Abs(distance.z) * Math.Tan(yAng * Math.PI/180)), 0);
        distance *= Math.Abs(distance.z)/distance.magnitude;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y + 180, -cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;

        var origSize = 54.8f;
        var neededSize = (float)Math.Tan(visualAngle * Math.PI/180 * 1/2) * distance.magnitude * 2;
        obj.transform.localScale = new Vector3(neededSize/origSize, neededSize/origSize, 1);
    }
    
    // Used to set the IPD to a certain amount (by the IPD stage)
    public static void changeIPDBy(float dx) {
        IPD += dx;
        IPD = Math.Max(0, IPD);
    }
}