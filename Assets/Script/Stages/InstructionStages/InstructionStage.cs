using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class InstructionStage : Stage {
    private float timeElapsed;
    public override void start() {
        StageStatic.GameObjects["startButton"].SetActive(false);
        StageStatic.GameObjects["instructions"].SetActive(true);
        StageStatic.GameObjects["target"].SetActive(false);
        timeElapsed = 0;
    }
    public override void update() {
        timeElapsed += Time.deltaTime;
        StageStatic.moveInstructionsTo(50);
    }
    public override bool finished() {
        return timeElapsed >= 10.0f;
    }
    public override void end() { }
}