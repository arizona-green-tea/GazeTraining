using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public static class StageVars {
    public static Dictionary<string, GameObject> GameObjects;
    public static Dictionary<string, AudioSource> Audios;
    public static ViveSR.anipal.Eye.EyeDataCol EyeDataCol;

    public static void setInformation(Dictionary<string, GameObject> allGameObjects, Dictionary<string, AudioSource> allAudio,
    ViveSR.anipal.Eye.EyeDataCol eyeDataCollection) {
        GameObjects = allGameObjects;
        Audios = allAudio;
        EyeDataCol = eyeDataCollection;
    }
}