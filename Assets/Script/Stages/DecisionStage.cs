using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;
public class DecisionStage : Stage {
    private Func<bool> useFirstStage;
    private Stage stage1, stage2, usedStage;
    public DecisionStage(Func<bool> useFirstStage, Stage stage1, Stage stage2) {
        this.useFirstStage = useFirstStage;
        this.stage1 = stage1;
        this.stage2 = stage2;
    }
    public override void start() {
        if (useFirstStage()) usedStage = stage1;
        else usedStage = stage2;

        usedStage.start();
    }
    public override void update() {
        if (!usedStage.finished()) usedStage.update();
    }
    public override bool finished() { return usedStage.finished(); }
    public override void end() { usedStage.end(); }
}