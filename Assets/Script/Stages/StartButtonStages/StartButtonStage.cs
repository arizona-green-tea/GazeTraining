using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class StartButtonStage : Stage {
    public override void start() {
        StageStatic.GameObjects["startButton"].SetActive(true);
        StageStatic.GameObjects["instructions"].SetActive(false);
        StageStatic.GameObjects["target"].SetActive(false);
    }
    public override void update() {
        StageStatic.moveStartButtonTo(60);
    }
    public override bool finished() {
        return (StageStatic.hasActiveUser && (StageStatic.EyeDataCol.lObjectName == "StartButton" || StageStatic.EyeDataCol.rObjectName == "StartButton")) 
            || Input.GetKeyDown(KeyCode.A);
    }
    public override void end() { }
}