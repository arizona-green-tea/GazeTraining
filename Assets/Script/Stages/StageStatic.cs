using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public static class StageStatic {
    public static Dictionary<string, GameObject> GameObjects;
    public static Dictionary<string, AudioSource> Audios;
    public static ViveSR.anipal.Eye.EyeDataCol EyeDataCol;
    public static Vector3 startingCameraPosition, startingCameraRotation;
    public static bool hasActiveUser = false;
    // Stores if the dartboard will be positioned relative to the world (val: true) or user (val: false)
    public static bool relativeToWorld = false;

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
    public static void moveStartButtonTo(float distance) {
        moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
            GameObjects["startButton"], new Vector3(0, 0, -distance));
    }

    // Handles moving the instructions to a certain distance away from the user
    public static void moveInstructionsTo(float distance) {
        moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
            GameObjects["instructions"], new Vector3(0, 0, -distance));
    }

    // Handles the movement of the dartboard
    // Inputs:
    // distance - the distance of the dartboard from the user
    // visualAngle - the size of the dartboard, in terms of visual angle (degrees)
    // xAngle - the horizontal position of the dartboard, in visual angle (degrees)
    // yAngle - the vertical position of the dartboard, in visual angle (degrees)
    public static void moveDartboardTo(float distance, float visualAngle, float xAngle, float yAngle) {
        if (!relativeToWorld) {
            moveToView(GameObjects["camera"].transform.position, GameObjects["camera"].transform.localRotation.eulerAngles,
                GameObjects["target"], new Vector3(0, 0, -distance), visualAngle, -xAngle, yAngle);
        } else {
            moveToView(startingCameraPosition, startingCameraRotation,
                GameObjects["target"], new Vector3(0, 0, -distance), visualAngle, -xAngle, yAngle);
        }
    }

    // Used for start button / instructions - moves them right in front of the user, at the specified distance
    private static void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance) {
        obj.transform.eulerAngles = cameraAng;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y + 180, cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;
    }

    // Used for dartboard - moves it to the specified location relative to the user
    private static void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance, float visualAngle, float xAng, float yAng) {
        if (xAng == 0 && yAng == 0) {
            moveToView(cameraPos, cameraAng, obj, distance);
        }
        
        obj.transform.LookAt(cameraPos);

        distance += new Vector3((float)(Math.Abs(distance.z) * Math.Tan(xAng * Math.PI/180)), (float)(Math.Abs(distance.z) * Math.Tan(yAng * Math.PI/180)), 0);
        distance *= Math.Abs(distance.z)/distance.magnitude;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y + 180, -cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;

        float origSize = 54.8f;
        float neededSize = (float)Math.Tan(visualAngle * Math.PI/180 * 1/2) * distance.magnitude * 2;
        obj.transform.localScale = new Vector3(neededSize/origSize, neededSize/origSize, 1);
    }
}