using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;
public class FutureStage : Stage {
    private Func<Stage> stage;
    private Stage actualStage;
    public FutureStage(Func<Stage> stage) {
        this.stage = stage;
    }
    public override void start() {
        actualStage = stage();
        actualStage.start();
    }
    public override void update() {
        if (!actualStage.finished()) actualStage.update();
    }
    public override bool finished() { return actualStage.finished(); }
    public override void end() { actualStage.end(); }
}